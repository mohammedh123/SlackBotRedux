using System.Data;
using System.Linq;
using Dapper;
using SlackBotRedux.Data.Interfaces;

namespace SlackBotRedux.Data.Sql
{
    public class VariableRepository : IVariableRepository
    {
        private readonly IDbConnection _conn;

        public VariableRepository(IDbConnection conn)
        {
            _conn = conn;
        }

        public ValidateAddingValueResult ValidateAddingValue(string variableName, string value)
        {
            // :based: simon :based:
            const string sql = @"
                WITH RecursiveValues AS
                (
                    SELECT V.Name, VV.VariableId, VV.Value, CAST(CONCAT('|', VV.VariableId, ',', VV.Value, '|') AS VARCHAR(MAX)) AS Visited
                    FROM VariableValues VV
                    JOIN Variables V ON V.Id = VV.VariableId
                    WHERE @value LIKE '%$' + V.Name + '%'
                    UNION ALL
                    SELECT V2.Name, v.VariableId, v.Value, CAST(CONCAT(r.Visited, v.VariableId, ',', v.Value, '|') AS VARCHAR(MAX)) AS Visited
                    FROM RecursiveValues r
                    JOIN VariableValues v ON v.Value LIKE '%$' + r.Name + '%'
                        AND r.Visited NOT LIKE CONCAT('%|', v.VariableId, ',', v.Value, '|%')
                    JOIN Variables V2 ON V2.Id = v.VariableId
                )
                SELECT CASE
                    WHEN EXISTS (SELECT 1 FROM VariableValues WHERE Value = @value)
                        THEN @alreadyExistsStr
                    WHEN NOT EXISTS (SELECT 1 FROM RecursiveValues) OR EXISTS (SELECT 1 FROM RecursiveValues WHERE Value NOT LIKE '%$' + @variableName + '%')
                        THEN @successStr
                    ELSE @recursiveStr
                END
            ";

            var paramz = new
            {
                variableName,
                value,
                successStr = ValidateAddingValueResult.Success,
                alreadyExistsStr = ValidateAddingValueResult.AlreadyExists,
                recursiveStr = ValidateAddingValueResult.Recursive,
            };

            var result = _conn.Query<ValidateAddingValueResult>(sql, paramz).Single();

            return result;
        }

        public ValidateDeletingValueResult ValidateDeletingValue(string variableName, string value)
        {
            // :based: simon :based:
            const string sql = @"
                WITH RecursiveValues AS
                (
                    SELECT V.Name, VV.VariableId, VV.Value, CAST(CONCAT('|', VV.VariableId, ',', VV.Value, '|') AS VARCHAR(MAX)) AS Visited
                    FROM VariableValues VV
                    JOIN Variables V ON V.Id = VV.VariableId
                    WHERE V.Name = @variableName
                    UNION ALL
                    SELECT V2.Name, v.VariableId, v.Value, CAST(CONCAT(r.Visited, v.VariableId, ',', v.Value, '|') AS VARCHAR(MAX)) AS Visited
                    FROM RecursiveValues r
                    JOIN VariableValues v ON v.Value LIKE '%$' + r.Name + '%'
                        AND r.Visited NOT LIKE CONCAT('%|', v.VariableId, ',', v.Value, '|%')
                    JOIN Variables V2 ON V2.Id = v.VariableId
                )
                SELECT CASE
                    WHEN NOT EXISTS (SELECT 1 FROM RecursiveValues) OR EXISTS (SELECT 1 FROM RecursiveValues WHERE Value NOT LIKE '%$' + @variableName + '%')
                        THEN @successStr
                    ELSE @recursiveStr
                END
            ";

            var paramz = new
            {
                variableName,
                value,
                successStr = ValidateDeletingValueResult.Success,
                recursiveStr = ValidateDeletingValueResult.Recursive,
            };

            var result = _conn.Query<ValidateDeletingValueResult>(sql, paramz).Single();

            return result;
        }
    }
}
