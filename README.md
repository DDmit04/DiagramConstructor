# DiagramConstructor

Console С# application creating visio diagrams from C++ code.

# How to use

* For start app download /DiagramConstructor/bin/Release directory (or whole repo) and run DiagramConstructor.exe 

* Choose code to build diagram (all code inside target method) this part will be improved later:

``` diff
- void main()
+       String code = "";
+       if (test)
+       {
+           code = "test code";
+       }
+       app.RunApp(code);
-       }
```

* Input cosen code in code converter (can be opened from console):

![Example2](/examples/Example1.png?raw=true)

convert it to one single line

![Example2](/examples/Example2.png?raw=true)

and input converted code in console

![Example3](/examples/Example3.png?raw=true)

* Look for "result.vsdx" in default documents folder of your PC.

# Recomendation for better app work

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

* Вon't use too complex "if" constructions

``` diff
- if(a == 10) 
- {
-     if(b == 20)
-     {
-         b = 0;
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

* "switch" and "do-while" constructions isn't supported now (probably "do-while" will nevwe be supported)
