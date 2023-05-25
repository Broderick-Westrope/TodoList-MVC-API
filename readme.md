# Todo List

## Use Cases

- [x] Create User
- [x] Delete User
- [x] Create TodoItem
- [x] Update TodoItem
- [x] Get a list of TodoItems
- [x] Delete TodoItem
- [x] Create Project
- [x] Delete Project
- [x] Create ProjectTodoItemMapping
- [ ] Delete ProjectTodoItemMapping
- [ ] Delete ProjectTodoItemMappings when corresponding Project is deleted
- [ ] Delete TodoItem when corresponding ProjectTodoItemMapping is deleted
- [ ] Delete Projects when corresponding User is deleted
- [ ] Delete Project when corresponding all ProjectTodoItemMappings are deleted
- [ ] Change User email
- [ ] Change User password

## Tests

```markdown
Given there is no user named "Brodie Westrope"
When I send a post request for a user named "Brodie Westrope"
Then a user is created with the name "Brodie Westrope"
```
