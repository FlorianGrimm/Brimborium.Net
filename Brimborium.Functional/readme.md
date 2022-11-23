# Brimborium Functional

## Brimborium.LocalObservability

This is an idea. I hope a usefull one ... at least for me.
I try to develop the idea, as the project process.

### Motivation:
good usefull log

### Story:

- depending on the article you read logging costs from 10% to a crazy number of I/O or CPU (loglevel, distributed,...).
- do you see your logs the first time if your customer send you a log-file?
- do you try to use logging instead of debugging because you cannot do this on this system and explode the log files?
- do you log each and every function entry and exit?
- do you use the anti-pattern log and rethrow because you have at least the stacktrace?

That's ok,

but what if you can use logging to enhance your unit tests, confidence to find wiered configuration or state or flows.

What I want to try:

use the stream of the log messages
apply filter, rules, projections, parition ...
and 
    - have convidence when you read the log
    - find bugs, 
    - and more if this works

Their are log analysis tools that can send a email, show graphs on a big screen that is not what I want.
Wouldn't it be nice to have an additional information about your program does what it is should do, and may be a customer can benevit from.

### Mechanics
Add another log provider to your program.
Filter the log and extract data - not only your call also the one from a framework or ...

### Define rules 

### an example to visualize the idea

this is a example log
```
request begin
noise
sql query
more noise
number of records found
request end
```

using the rule:

```
request begin 
> verify the web api arguments 
> sql query
```

will fail because their is no verification step - yeah surely you didn't do it, because the next rollout was the next days and you fix it later...

Hitting your unit, e2e or isolated - tests - what if you don't have to fake a sqlserver or your most hated standard/proporiary monster software to have validation that the code works as expected.

May be the person which have to read "your" logs have additional evidence why the line are falsy and hit the problem.

### code and tooling
Write the code, log the right things, analyse the log build rules.
Running the program (with an flag, configuration),... and verify the log against the rules.
Analyse the logs and try to find a rule to verify the correctness and narrow down the bugs.

### Limits

Since the logs are expensive and logs should not contain any security relevat things, these limits will limit the usage for different rules, but I hope their might be suprisingly usefull.

### No idea how

Since the focus is for developer the rules can be C#.
Which might not help the admins.

A dsl might be usefull for off loading the rule analysis to an different process.

Decoupling might be good and bad.



