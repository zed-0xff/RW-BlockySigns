Test_ExpCompiler.Run();
Test_Extensions.Run();

var c = Console.ForegroundColor;
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("[=] ALL OK!");
Console.ForegroundColor = c;
