# Remote Dictionary Service

## Overview
- **Primary Focus**: Speed. It is used when extremely high speed is required.
- A type of NoSQL database designed in a **key:value** format. It is utilized when more than just a key-value pair is needed.
- **Optional Disk Storage**: Redis can write to disk optionally. It is often used where the dataset is not critical. Redis stores data in memory, which imposes significant memory constraints. 
  - **Memory Usage**: 
    - An empty instance uses 1 MB of memory. 
    - 1 million key-value string pairs use approximately 100 MB of memory.

## Supported Data Types
1. **String**
2. **Hash**
3. **List**
4. **Set**
5. **Sorted Set**
6. **Geospatial Index**
7. **HyperLogLog**

## Use Cases
1. **Database**
2. **Caching Layer**
3. **Message Broker**

## Where It Should Not Be Used
- Redis is not a relational database. However, it can be used as a layer to define relationships for such database types.
- Its best use case is handling datasets with a predictable size that grows rapidly, requiring speed.

---

## Usage Scenarios
- **Caching Mechanisms**
- **Pub/Sub**
- **Preventing and Delaying Queues**
- **Short-lived Data**
- **Counting Comments**
- **Analyzing Real Data**
- **Storing Unique Data**

---

## Scaling Redis

### Persistence
Redis provides two persistence mechanisms:
1. **RDB (Redis Database Snapshots)**
2. **AOF (Append-Only Files)**

### Replication
- A single Redis instance acts as the **master**.
- Other instances are **slaves**, serving as copies of the master.
- Clients can connect to either master or slave instances.
- By default, slaves are read-only.

### Partitioning
- Data can be split and distributed across instances.

### Failover
- **Manual**
- **Automatic**: Managed by **Redis Sentinel** for master-slave topologies.

---

## Example: Setting up Redis with .Net Core

# Running a Redis Container

To start a Redis container using Docker, execute the following command:

```bash
docker run --name my-redis -p 6379:6379 -d redis
```
### Create a Web API Project
```bash
dotnet new webapi -n "CachingWebApi"
```

![Screenshot 6](https://github.com/ilkersatur/RedisCache-PostgreSql-Docker-.NetCore/blob/main/RedisCache.png)

![Screenshot 7](https://github.com/ilkersatur/RedisCache-PostgreSql-Docker-.NetCore/blob/main/RedisCacheDocker.png)
