Test_ExpCompiler.Run();
Test_Extensions.Run();
Test_Null.Run();

var c = Console.ForegroundColor;
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("[=] ALL OK!");
Console.ForegroundColor = c;
