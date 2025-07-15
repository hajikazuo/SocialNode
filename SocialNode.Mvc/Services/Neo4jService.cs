using Microsoft.Extensions.Options;
using Neo4j.Driver;
using SocialNode.Mvc.Settings;

namespace SocialNode.Mvc.Services
{
    public class Neo4jService : INeo4jService, IDisposable
    {
        private readonly IDriver _driver;
        private readonly string _database;

        public Neo4jService(IOptions<Neo4jSettings> settings)
        {
            var config = settings.Value;
            _database = config.Database ?? "neo4j";
            _driver = GraphDatabase.Driver(config.Uri, AuthTokens.Basic(config.Username, config.Password));
        }

        public async Task CreateUserAsync(Guid userId, string fullName)
        {
            var query = @"
                MERGE (u:User {id: $id})
                SET u.fullName = $fullName";

            var parameters = new
            {
                id = userId.ToString(),
                fullName
            };

            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(query, parameters);
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating user in Neo4j", ex);
            }
        }

        public async Task CreateFriendshipAsync(Guid userId1, Guid userId2)
        {
            var query = @"
                MATCH (a:User {id: $id1}), (b:User {id: $id2})
                MERGE (a)-[:FRIEND_OF]->(b)
                MERGE (b)-[:FRIEND_OF]->(a)";

            var parameters = new
            {
                id1 = userId1.ToString(),
                id2 = userId2.ToString()
            };

            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(query, parameters);
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating friendship in Neo4j", ex);
            }
        }

        public async Task<List<Guid>> GetFriendsAsync(Guid userId)
        {
            var query = @"
                MATCH (u:User {id: $id})-[:FRIEND_OF]->(friend:User)
                RETURN friend.id AS friendId";

            var parameters = new 
            {
                id = userId.ToString() 
            };
            var friendIds = new List<Guid>();

            await using var session = _driver.AsyncSession();
            var result = await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(query, parameters);
                var records = await cursor.ToListAsync();

                foreach (var record in records)
                {
                    if (Guid.TryParse(record["friendId"].As<string>(), out var friendId))
                    {
                        friendIds.Add(friendId);
                    }
                }

                return friendIds;
            });

            return result;
        }

        public async Task<List<Guid>> GetFriendSuggestionsAsync(Guid userId)
        {
            var query = @"
                MATCH (u:User {id: $id})-[:FRIEND_OF]->(:User)-[:FRIEND_OF]->(suggested)
                WHERE NOT (u)-[:FRIEND_OF]->(suggested) AND u <> suggested
                RETURN DISTINCT suggested.id AS suggestedId
                LIMIT 10";

            var parameters = new
            {
                id = userId.ToString()
            };

            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            try
            {
                var result = await session.ExecuteReadAsync(async tx =>
                {
                    var cursor = await tx.RunAsync(query, parameters);
                    return await cursor.ToListAsync(record =>
                    {
                        var idStr = record["suggestedId"].As<string>();
                        return Guid.TryParse(idStr, out var guid) ? guid : Guid.Empty;
                    });
                });

                return result.Where(g => g != Guid.Empty).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving friend suggestions", ex);
            }
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}
