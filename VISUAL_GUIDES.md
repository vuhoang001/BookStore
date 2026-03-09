# 📊 VISUAL GUIDES - INTEGRATIONEVENTS ARCHITECTURE

## 1️⃣ EVENT DISPATCH TIMELINE

```
TIME ──────────────────────────────────────────────────────────────►

t₀: Book.Create() called
    └─ Constructor runs
       └─ RegisterDomainEvent(new BookCreatedEvent(this)) ✅

t₁: SaveEntitiesAsync()
    └─ DbContext.SaveChangesAsync()
       │
       ├─ INTERCEPTOR PHASE: EventDispatchInterceptor.SavingChangesAsync()
       │  ├─ Before SaveChanges executes
       │  ├─ Collect entities with domain events
       │  ├─ Call MediatorDomainEventDispatcher.DispatchAndClearEvents()
       │  │  └─ Publish each DomainEvent via IPublisher (Mediator)
       │  │     └─ INotificationHandler<BookCreatedEvent> receives
       │  │        └─ BookCreatedEventHandle.Handle()
       │  │           └─ Just log, NO dispatch ✅
       │  └─ Return from interceptor
       │
       └─ SAVE PHASE: DbContext.SaveChanges()
          ├─ Insert/Update/Delete data
          ├─ Create OutboxMessage entry
          │  └─ Contains: BookCreateIntegrationEvent (serialized)
          └─ Commit transaction ✅

t₂: Return from SaveEntitiesAsync() ✅

t₃: (Background) Outbox Service running
    └─ Timer triggers every QueryDelay (5 seconds)
       ├─ Query OutboxMessage where not published
       ├─ For each message:
       │  └─ Call EventDispatcher.DispatchAsync(domainEvent)
       │     ├─ EventMapper.MapToIntegrationEvent()
       │     ├─ IBus.Publish(integrationEvent)
       │     │  └─ Message added to RabbitMQ queue
       │     └─ Mark OutboxMessage as published
       └─ Continue polling

t₄: RabbitMQ receives message
    └─ Message in queue: book-create-integration-event

t₅: Consumer connects (IConsumer<BookCreateIntegrationEvent>)
    └─ BookCreateIntegrationHandler.Consume()
       └─ Process the event
```

---

## 2️⃣ DOUBLE DISPATCH PROBLEM vs SOLUTION

### ❌ BEFORE (WRONG):
```
Book Constructor
    │
    └─► RegisterDomainEvent(BookCreatedEvent)
        │
        └─► SaveChangesAsync()
            │
            ├─► EventDispatchInterceptor
            │   └─► MediatorDomainEventDispatcher
            │       └─► INotificationHandler<BookCreatedEvent>
            │           └─► BookCreatedEventHandle
            │               │
            │               ├─► Dispatch #1: eventDispatcher.DispatchAsync() ❌
            │               │   └─► RabbitMQ: Message #1 added
            │               │
            │               └─► Log: "Finished dispatching"
            │
            ├─► SaveChanges()
            │   └─► OutboxMessage saved
            │
            └─► Outbox Service
                └─► EventDispatcher.DispatchAsync()
                    │
                    ├─► Dispatch #2: IBus.Publish() ❌
                    │   └─► RabbitMQ: Message #2 added (DUPLICATE!)
                    │
                    └─► Mark as published

RESULT: 2 messages in queue (Duplicate event processing!)
```

### ✅ AFTER (CORRECT):
```
Book Constructor
    │
    └─► RegisterDomainEvent(BookCreatedEvent)
        │
        └─► SaveChangesAsync()
            │
            ├─► EventDispatchInterceptor
            │   └─► MediatorDomainEventDispatcher
            │       └─► INotificationHandler<BookCreatedEvent>
            │           └─► BookCreatedEventHandle
            │               │
            │               └─► Just log, NO dispatch ✅
            │
            ├─► SaveChanges()
            │   └─► OutboxMessage saved
            │
            └─► Outbox Service (Background)
                └─► EventDispatcher.DispatchAsync()
                    │
                    └─► SINGLE Dispatch: IBus.Publish() ✅
                        └─► RabbitMQ: Single message added

RESULT: 1 message in queue (Correct single dispatch!)
```

---

## 3️⃣ DATA FLOW COMPARISON

### ❌ BEFORE:
```
Command
  │
  └─ Handler (imports EventBus, IntegrationEvents, MassTransit)
     └─ Creates Book
        └─ SaveChanges
           └─ Handler dispatches AGAIN
              └─ Duplicate in queue
```

### ✅ AFTER:
```
HTTP POST /book
  │
  ├─► CreateBookCommand
  │   └─ CLEAN: No event imports
  │
  ├─► CreateBookCommandHandler
  │   └─ 1. Create Book aggregate
  │   └─ 2. Call SaveEntitiesAsync()
  │
  ├─► EventDispatchInterceptor
  │   └─ (Automatic - before SaveChanges)
  │   └─ Dispatch domain events via Mediator
  │
  ├─► Handlers (INotificationHandler)
  │   └─ BookCreatedEventHandle
  │      └─ React locally (NO dispatch!)
  │
  ├─► SaveChanges
  │   └─ Persist Book + OutboxMessage
  │
  ├─► Outbox Service (Background)
  │   └─ SINGLE dispatch point
  │   └─ Domain Event → Integration Event
  │   └─ Publish to RabbitMQ
  │
  ├─► RabbitMQ Consumer
  │   └─ BookCreateIntegrationHandler
  │      └─ Process event
```

---

## 4️⃣ MESSAGE STRUCTURE IN QUEUE

### BookCreateIntegrationEvent
```json
{
  "messageId": "abc123...",
  "sentTime": "2026-03-09T10:30:00Z",
  "headers": {
    "Message-Id": "abc123...",
    "Content-Type": "application/json"
  },
  "body": {
    "id": "def456...",                 // IntegrationEvent.Id
    "creationDate": "2026-03-09T10:29:59Z",  // IntegrationEvent.CreationDate
    "bookId": "111-222-333-444",       // Book identifier
    "name": "The Great Gatsby",
    "description": "A classic novel...",
    "price": 9.99,
    "priceSale": null,
    "categoryId": "cat-001",
    "publisherId": "pub-001",
    "authorIds": ["auth-001", "auth-002"]
  }
}
```

---

## 5️⃣ DATABASE STATE

### OutboxMessage Table (SQL Server)
```sql
┌─────────────────────────────────────────────────────────────┐
│ OutboxMessage                                               │
├─────────────────────────────────────────────────────────────┤
│ SequenceNumber  │ 1                                         │
│ EnqueueTime     │ 2026-03-09 10:30:00                       │
│ SentTime        │ 2026-03-09 10:29:59                       │
│ MessageType     │ BookCreateIntegrationEvent                │
│ InboxMessageId  │ NULL                                      │
│ Properties      │ { ... metadata ... }                      │
│ Content         │ {"bookId":"...","name":"..."}             │
└─────────────────────────────────────────────────────────────┘

State during processing:
1. Created when SaveChanges executes
2. Picked up by Outbox Service poll
3. Published to RabbitMQ
4. Marked as delivered
5. Eventually deleted (cleanup)
```

### Book Table
```sql
┌──────────────────────────────────────────────────────────────┐
│ Books                                                         │
├──────────────────────────────────────────────────────────────┤
│ Id              │ 111-222-333-444                           │
│ Name            │ The Great Gatsby                          │
│ Description     │ A classic novel...                        │
│ Price           │ 9.99                                      │
│ PriceSale       │ NULL                                      │
│ CategoryId      │ cat-001                                   │
│ PublisherId     │ pub-001                                   │
│ CreatedAt       │ 2026-03-09 10:30:00                       │
│ UpdatedAt       │ 2026-03-09 10:30:00                       │
└──────────────────────────────────────────────────────────────┘
```

---

## 6️⃣ HANDLER EXECUTION ORDER

```
When SaveChangesAsync() called:

Step 1: Book Added to ChangeTracker
        ├─ Book.Id = Guid.NewGuid()
        ├─ Book.DomainEvents = [BookCreatedEvent]
        └─ Status = EntityState.Added

Step 2: EventDispatchInterceptor.SavingChangesAsync (Before SaveChanges)
        ├─ Read entities with DomainEvents
        │  └─ Finds: Book with [BookCreatedEvent]
        │
        ├─ Call MediatorDomainEventDispatcher.DispatchAndClearEvents()
        │  ├─ For each event: publisher.Publish(BookCreatedEvent)
        │  │  └─ Mediator finds handlers
        │  │     └─ BookCreatedEventHandle.Handle() called ✅
        │  │
        │  └─ Clear DomainEvents collection
        │
        └─ Return to SaveChanges

Step 3: DbContext.SaveChanges()
        ├─ Generate INSERT for Book
        ├─ Generate INSERT for OutboxMessage (from MassTransit)
        │  └─ Contains serialized BookCreateIntegrationEvent
        │
        └─ COMMIT TRANSACTION ✅
           └─ Both Book and OutboxMessage persist atomically

Step 4: (Later) Outbox Service
        ├─ Timer fires
        ├─ SELECT from OutboxMessage where Delivered = false
        ├─ EventDispatcher.DispatchAsync(event)
        │  └─ Publish to IBus
        │     └─ Message added to RabbitMQ
        │
        └─ UPDATE OutboxMessage set Delivered = true
```

---

## 7️⃣ EXCEPTION HANDLING FLOW

### Scenario: SaveChanges fails
```
Book created and DomainEvent registered
    │
    ├─► EventDispatchInterceptor dispatches (via Mediator)
    │   ├─ In-memory only
    │   └─ Mediator publishes to local handlers
    │
    ├─► SaveChanges FAILS! ❌
    │   ├─ Exception thrown
    │   ├─ Transaction rolled back
    │   └─ Outbox NOT created
    │
    └─► Exception caught by caller
        └─ Request fails
           └─ Message NEVER queued ✅ (Correct!)

→ No orphaned messages, no ghost events
```

### Scenario: Handler throws exception
```
EventDispatchInterceptor calls handlers
    │
    └─► BookCreatedEventHandle.Handle() throws exception
        │
        ├─ Exception propagates
        ├─ SaveChanges NEVER called
        ├─ Transaction NOT committed
        └─ Request fails
           └─ Entire operation rolled back ✅

→ Transactional consistency maintained
```

---

## 8️⃣ MONITORING CHECKLIST

### What to Monitor
```
1. OutboxMessage Table
   □ Row count increasing (messages being created)
   □ Delivered flag changing to true (messages being processed)
   □ No old undelivered messages (> 1 hour old)

2. RabbitMQ Queue
   □ Message count
   □ Message age
   □ Dequeue rate
   □ Dead-letter queue

3. Application Logs
   □ EventDispatcher: "Successfully published"
   □ Handlers: "Consumed message"
   □ No duplicate handler logs for same event

4. Performance
   □ Save operation time
   □ Outbox service poll frequency
   □ Message processing latency
   □ Error rates
```

### Example Monitoring Query
```sql
-- Check Outbox processing
SELECT 
    COUNT(*) as TotalMessages,
    SUM(CASE WHEN Delivered = 0 THEN 1 ELSE 0 END) as Pending,
    SUM(CASE WHEN Delivered = 1 THEN 1 ELSE 0 END) as Delivered,
    AVG(DATEDIFF(SECOND, EnqueueTime, SentTime)) as AvgProcessTimeSeconds
FROM OutboxMessage
WHERE SentTime > DATEADD(HOUR, -1, GETUTCDATE())
```

---

## 9️⃣ CONFIGURATION DETAILS

### MassTransit Configuration
```csharp
cfg.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
{
    o.QueryDelay               = TimeSpan.FromSeconds(5);   // Poll every 5 seconds
    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);   // Detect dups in 5 min window
    o.UseSqlServer();                                        // SQL Server outbox
    o.UseBusOutbox();                                        // Use bus for publishing
});
```

### Consumer Registration
```csharp
cfg.AddConsumer<BookCreateIntegrationHandler>();
cfg.AddConsumer<BookUpdateIntegrationHandler>();
cfg.AddConsumer<BookChangeIntegrationHandler>();
```

### RabbitMQ Endpoints
```
book-create-integration-event     ← BookCreateIntegrationEvent
book-update-integration-event     ← BookUpdateIntegrationEvent
book-change-integration-event     ← BookChangeIntegrationEvent
```

---

## 🔟 TRANSACTION GUARANTEE

### Exactly-Once Delivery Promise
```
                 Application DB                    Message Queue
                 ─────────────                      ─────────────

Step 1:   ┌─ BEGIN TRANSACTION
          │

Step 2:   ├─ INSERT Book
          │

Step 3:   ├─ INSERT OutboxMessage
          │
          └─ COMMIT TRANSACTION ─────────────────────► Message logged

Step 4:   (Later) Outbox Service
          │
          ├─ PUBLISH to RabbitMQ ────────────────────► Message queued
          │
          └─ UPDATE OutboxMessage (Delivered=true)

GUARANTEE:
✅ If SaveChanges fails → No OutboxMessage created → No message sent
✅ If Message already published → Duplicate detection prevents reprocessing
✅ If Consumer fails → Message redelivered (with retry policy)
✅ Exactly-once per Message-Id
```

---


