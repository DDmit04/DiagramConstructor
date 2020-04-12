# DiagramConstructor

Console С# application creating visio diagrams from C++ code.

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

* Then simpler and cleaner the code you wrote, the easier it is to create a diagram.

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

* "switch" and "do-while" constructions isn't supported now. Replace them by one block, and add this constructions manualy after application do work.

# Bugs and errors

Send all bugs and errors (with console screen, inputed code and result diagram) in "issues" section of this repository.
