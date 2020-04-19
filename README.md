# DiagramConstructor (1.0.2.)

Console С# application creating visio diagrams from C++ code.

[See patch notes](/PATCH_NOTES.md)

# How to use

* For start app download /DiagramConstructor/bin/Release directory (or whole repo) and run DiagramConstructor.exe 

* Choose code to build diagram (you can copy all file):

![Example1](/examples/Example1.png?raw=true)


* Input cosen code in code converter which can be opened from app console:

![Example2](/examples/Example2.png?raw=true)


* convert it to one single line

![Example3](/examples/Example3.png?raw=true)


* write result file path (optional)

![Example4](/examples/Example4.png?raw=true)


* and input converted code in console

![Example5](/examples/Example5.png?raw=true)


* Look for .vsdx file in inputed file path or default documents folder of your PC.

# Recomendation for better app work

* You need licensed or cracked Visio (you can try run visioActivator.cmd in root of this repo with admin rights to get license).

* Then simpler and cleaner the code you wrote, the easier it is to create a diagram.

* Use only english words in code (even in comments).

* Init varible before use it (optional).
```diff
- int a = someValue;

+ int a = 0;
+ a = somValue;
```

* Code to input in console must be converted (no spaces, tabs and line breaks):

``` diff

- String code = "";
- if (test)
- {
- code = "test code";
- }
- app.RunApp(code);

+ Stringcode='';if(test){code='';}app.RunApp(code);
```

* Use common "for" instead "foreach"
```dif
- foreach(SomeType value in array) {}

+ for(int i = 0; i < array.length; i++) {}
```

* Use parentheses even for single line code blocks:

``` diff
- if(a > 10)
-    a = 10;
- else
-    a = 0;

- for(int i = 0; i < 10; i++) 
-     count++;

+ if(a > 10)
+ {
+    a = 10;
+ }
+ else
+ {
+    a = 0;
+ }

+ for(int i = 0; i < 10; i++)
+ {
+     count++;
+ }
```

* Don't use '{' , '}' and ';' chars in code as parts of strings;

``` diff
- String myString = "if(a == 0) { int a = 10; }"; 
```

* Don't use too complex "if" constructions

``` diff
- if(a == 10) 
- {
-     if(b == 20)
-     {
-         if(c == 0)
-         {
-             c == 0;
-         }
-     }
-     else
-     {
-         b = 100;
-     }
- }
- else 
- {
-     a = 0;
- }
```

* Use only standart "else if" form:

``` diff
- if(a == 10)
- {
-     a--;
- }
- else 
- { 
-     if(a == 20)
-     {
-         a--;
-     }

+ if(a == 10)
+ {
+     a--;
+ }
+ else if(a == 20)
+ {
+     a--;
+ }
```

# Unsupported

* "Break" and "continue" - they will be placed as PROCESS blocks

* "switch" - use "else if" instead of it

* "do-while" - replace it by one block, and add this constructions manualy after application do work.

# Example

From this code:
``` C++
#include <iostream>

using namespace std;

main()
{
    int a = 10;
    for (int i = 0; i < a; i++)
    {
        if (a * 2 == 4)
        {
            cout << "some output";
        }
        else
        {
        cout << "othet output";
        }
    }
}

doSome(int a, int b)
{
    if(a > b)
    {
        while(a != b)
        {
            a--;
        }
    } 
    else
    {
        while (a != b)
        {
            b--;
        }
    }
    cout << "justice";
}
```

you can get this diagram:

![Example6](/examples/Example6.png?raw=true)

# Bugs and errors

Send all bugs and errors (with console screen, inputed code and result diagram) in "issues" section of this repository.
