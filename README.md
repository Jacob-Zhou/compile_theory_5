# CUMT 编译原理 实验五 #

文件结构:
\IR wiki.txt : 中间代码的简介;
\IRSystem.png : 解释器的结构;
\样例.txt : 源代码样例;

\bin : 可执行文件;
\compile_theory_5 : 工程代码;

\compile_theory_5\Model\IRSystem : 中间代码解释器;

\compile_theory_5\Model\Compile.cs : 生成中间代码;
\compile_theory_5\Model\Error.cs : 错误类;
\compile_theory_5\Model\Lexer.cs : 词法分析器;
\compile_theory_5\Model\Parser.cs : 语法分析器;
\compile_theory_5\Model\Symbol.cs : 语法树类;
\compile_theory_5\Model\Token.cs : 词法单元类

\compile_theory_5\ViewModel\ErrorViewModel.cs : 在界面上显示错误信息;
\compile_theory_5\ViewModel\SourceViewModel.cs : 在界面上显示源代码;

\compile_theory_5\MainWindow.xaml : 主界面描述;
\compile_theory_5\MainWindow.xaml.cs : 窗口交互;