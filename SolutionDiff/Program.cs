using System.Text.RegularExpressions;
using Microsoft.Build.Construction;

Regex solutionR = new("^.*\\.sln$");
Regex projectR = new("^.*\\.csproj$|^.*\\.wixproj$");
string? solution = string.IsNullOrWhiteSpace(args[0]) ? Array.Find(Directory.GetFiles(Environment.CurrentDirectory), s => solutionR.IsMatch(s)):
                       args[0];

if (solution is null)
{
    Console.Error.WriteLine("No Solution found!");
    return;
}

List<string> projects = [];
Array.ForEach(SolutionFile.Parse(solution).ProjectsInOrder.ToArray(), T =>
{
    projects.Add(T.AbsolutePath);
});


Console.WriteLine("---------------------------------------------");


List<string> foundProjects = FindProjectsRecursive(Environment.CurrentDirectory);

foreach (string s in foundProjects.Where(s => projects.All(project => project != s)))
{
    Console.WriteLine(s);
}

List<string> FindProjectsRecursive(string folderPath)
{
    List<string> foundProjects = new();
    string? projectFile = Array.Find(Directory.GetFiles(folderPath), s => projectR.IsMatch(s));
    
    if (projectFile != null) foundProjects.Add(projectFile);
    Array.ForEach(Directory.GetDirectories(folderPath), s => foundProjects.AddRange(FindProjectsRecursive(s)));

    return foundProjects;
}