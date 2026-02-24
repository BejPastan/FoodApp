## Variables
when using varaibles, avoid when possible doingmath inside Query, do it inside binding, or make variable

good example
```
var sql = "SELECT * FROM tags WHERE name LIKE @name ORDER BY id ASC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";
return DBConnector.QueryDatabase<Tag>(sql, new { name = $"%{nameFilter}%", offset = (page - 1) * pageSize, pageSize = pageSize }).ToList();
```

bad example
```
var sql = "SELECT * FROM tags WHERE name LIKE @name ORDER BY id ASC OFFSET (@page - 1) * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY;";
return DBConnector.QueryDatabase<Tag>(sql, new { name = $"%{nameFilter}%", page = page, pageSize = pageSize }).ToList();
```

## Pagination
pagination in repository shuold always get values, it cannot have default values