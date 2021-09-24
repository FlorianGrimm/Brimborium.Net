# Brimborium.CodeBlocks

dotnet core 5

work in progress

cd Brimborium.Net\Brimborium.CodeBlocks

dotnet build

dotnet tool install --tool-path dotnetcodeblocks --add-source ./nupkg --version 1.0.5-beta-ga769bb6dcc dotnetcodeblocks -v d

dotnet tool install --tool-path dotnetcodeblocks --add-source ./nupkg dotnetcodeblocks -v d

dotnet tool uninstall --tool-path dotnetcodeblocks dotnetcodeblocks


dotnet run --project Brimborium.CodeBlocks -- install


dotnet codeblocks

config file


idea

source of data + template + extensionplaceholder + generator + old code -> new code
template language independend? dependend?
extensionplaceholder replaces content between
template
```
// start-usefullName replace
content-p1 
// start-foo default
content-p2
// stop-foo
// start-hugo replace
content-p3
// stop-hugp
content-p4
{{replacement-gna}}
content-p5
// stop-usefullName
```

as tree
```
CBTemplate(usefullName)
    - mode: replace
    - value 
        - CBStringFixed: content-p1
        - CBTemplate(foo)
            - mode: default
            - value
                - CBValueString: content-p2
        - CBTemplate(hugo)
            - mode: replace
            - value
                - CBValueString: content-p3
        - CBStringFixed: content-p4
        - CBPlaceholder(replacement-gna)
        - CBStringFixed: content-p5
```

old code
```
// start-usefullName-id1
old-content-p1
// start-foo-id2
old-content-p2
// stop-foo-id2
// start-hugo-id3
old-content-p3
// stop-hugo-id3
old-content-p4
replacement-gna
old-content-p5
// stop-usefullName-id1
```

old code as tree
```
CBReplaced(usefullName,id1)
    - value 
        - CBValueString: old-content-p1
        - CBReplaced(foo,id2)
            - CBValueString: old-content-p2
        - CBReplaced(hugo,id3)
            - CBValueString: old-content-p2
        - CBValueString: old-content-p3 old-content-p4 old-content-gna old-content-p5
```

data
```
data
    - hugo
        old-content-hugo -> new-content-hugo
    - gna
        replacement-gna
```

Template as tree - process
1. Template
```
CBTemplate(usefullName)
    - mode: replace
    - value 
        - CBStringFixed: content-p1
        - CBTemplate(foo)
            - mode: default
            - value
                - CBValueString: content-p2
        - CBTemplate(hugo)
            - mode: replace
            - value
                - CBValueString: content-p3
        - CBStringFixed: content-p4
        - CBPlaceholder(replacement-gna)
        - CBStringFixed: content-p5
```

2. Apply usefullName-id1
```
CBReplaced(usefullName,id1)
    - CBTemplate usefullName
    - value 
        - CBStringFixed: new-content-p1
        - CBTemplate(foo)
            - mode: default
            - value
                - CBValueString: content-p2
        - CBTemplate(hugo)
            - mode: replace
            - value
                - CBValueString: content-p3
        - CBStringFixed: content-p4
        - CBPlaceholder(replacement-gna)
        - CBStringFixed: content-p5
```

2. Apply foo-id2
```
CBReplaced(usefullName,id1)
    - CBTemplate usefullName
    - value 
        - CBStringFixed: new-content-p1
        - CBReplaced(foo,id2) - CBTemplate(foo)
            - CBValueString: old-content-p2

        - CBTemplate(foo)
            - mode: default
            - value
                - CBValueString: content-p2
        - CBTemplate(hugo)
            - mode: replace
            - value
                - CBValueString: content-p3
        - CBStringFixed: content-p4
        - CBPlaceholder(replacement-gna)
        - CBStringFixed: content-p5
```

3. Apply hugo-id3
```
CBReplaced(usefullName,id1)
    - CBTemplate usefullName
    - value 
        - CBStringFixed: new-content-p1
        - CBReplaced(foo,id2) - CBTemplate(foo)
            - CBValueString: old-content-p2

        - CBReplaced(hugo,id3)
            - CBValueString: new-content-hugo

        - CBStringFixed: content-p4
        - CBPlaceholder(replacement-gna)
        - CBStringFixed: content-p5
```

new code as tree

```
CBReplaced(usefullName,id1)
    - value 
        - CBValueString: new-content-p1
        - CBReplaced(foo,id2)
            - CBValueString: old-content-p2
        - CBReplaced(hugo,id3)
            - CBValueString: new-content-hugo
        - CBStringFixed: content-p4
        - CBPlaceholder(replacement-gna)
        - CBStringFixed: content-p5

        - CBValueString: old-content-p3 old-content-p4 old-content-gna old-content-p5

```

new code
```
// start-usefullName-id1
old-content-p1
// start-foo-id2
old-content-p2
// stop-foo-id2
// start-hugo-id3
new-content-hugo
// stop-hugo-id3
old-content-p4
replacement-gna
old-content-p5
// stop-usefullName-id1
