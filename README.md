# SlackBotRedux

A bot that integrates with Slack via the RTM API.

## Roadmap (WIP)
### Bot Infrastructure
- ~~Connecting to Slack via RTM API~~
- Ping heartbeat
- Rate-limit output queue
- ~~Sending and receiving different Slack messages~~
- Log everything (but censor anything said in private groups/DMs)

### Modules
#### Quotes
- `remember` command (remember stuff people say)
- `quotes` command (lists all quotes for a given user)

#### Variables
- ~~Allow circular references~~
- ~~Allow referencing of other variables, including the same variable~~
- ~~Allow protection of variables~~
- Magic/special variables (such as $band, $tla, $item, $who, etc)
- ~~`add variable` command~~
- `remove variable` command
- ~~`add value` command~~
- ~~`remove value` command~~
- `list values` command
- `list vars` command
- Variable types (noun, verb, var)

#### Factoids
- Allow referencing variables
- Allow protection of factoids
- Magic/special factoids (such as "don't know", "band name reply", etc)
- `X <verb> Y` command (including `<action>`)
- `list factoids` command (aka `literal X`)
- Triggering a specific factoid (aka `X ~= /text here/`)
- Substitution editing (aka `X ~= s/text to replace/new text/`)
- `what was that` command
- `undo last` command (only the one who made the last change can do this)
- `alias` command
- `merge` command
- `delete` command
- `delete id` command
- `forget that` command
- `lookup` command

#### Inventory
- various `gives item` command
- `delete item` command
- `list items` command

#### People
- Gender commands/variables
- Keeping track of recently speaking users
- Muting users/commands
- Permissions (op vs non-op)

#### Commands
- `say` command
- `shut up` command

#### TLAs
- Detecting TLAs

#### Config
- `set/get` commands
- Various configuration variables (band_name, band_var, etc)
