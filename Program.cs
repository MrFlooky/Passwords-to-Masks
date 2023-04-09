using System.Text.RegularExpressions;

long GetLinesCount(string path) {
    long i = 0;
	using (StreamReader sr = new(path))
		while (sr.ReadLine() != null)
			i += 1;
	return i;
}

HashSet<string> WriteToHash(string fileName) {
	string? tmpLine;
	HashSet<string> list = new((int)GetLinesCount(fileName));
	using (StreamReader reader = new(fileName))
		while ((tmpLine = reader.ReadLine()) != null)
			list.Add(tmpLine);
	return list;
}

bool IdentifyFileType(string path) {
	HashSet<string> lines = WriteToHash(path);
	double i = 0;
	foreach (string line in lines)
		if (line.Contains(':'))
			i++;
    long length = GetLinesCount(path);
	bool type = false;
	if (i / length > 0.7) type = true;
	return type;
}

HashSet<string> RemoveLogin(HashSet<string> initial) {
	HashSet<string> result = new();
	foreach (string line in initial)
		try {
			result.Add(line.Split(':')[1]);
		} catch {
            result.Add(line);
        }
	return result;
}

void MainWork(string path, float maxDifficulty) {
    HashSet<string> lines = WriteToHash(path);
    HashSet<string> resultMasks = new();
	if (File.Exists("masks.txt"))
		File.Delete("masks.txt");
    if (IdentifyFileType(path))
		lines = RemoveLogin(lines);
	string symbols = @" !""#$%&'()*+,-./:;<=>?@[\]^_`{|}~";
	foreach (string line in lines) {
        double difficulty = 1;
		char[] chars = line.ToCharArray();
		foreach (char c in chars) {
            if (char.IsUpper(c) || char.IsLower(c)) difficulty *= 26;
            else if (char.IsDigit(c)) difficulty *= 10;
            else if (symbols.Contains(c)) difficulty *= 33;
			else difficulty *= 161;
            if (difficulty > maxDifficulty || difficulty < 0) break;
        }
        if (difficulty > maxDifficulty || difficulty < 0) continue;
        string mask = "";
        foreach (char c in chars) {
            if (char.IsUpper(c)) mask += "?u";
            else if (char.IsLower(c)) mask += "?l";
            else if (char.IsDigit(c)) mask += "?d";
            else if (char.IsSymbol(c)) mask += "?s";
            else mask += "?b";
        }
		if (!resultMasks.Contains(mask)) {
			resultMasks.Add(mask);
			File.AppendAllText("masks.txt", $"{mask}\n");
		}
	}
	//if (File.Exists("masks.txt")) { }

}

while (true) {
	Console.WriteLine("Load a file ( log:pass / pass ): ");
	string? path = Console.ReadLine().Replace("\"", "");
	if (!(pathRegex().IsMatch(path) && File.Exists(path))) {
		Console.WriteLine("Drop valid file that exists.");
		continue;
	}
    Console.WriteLine("What max variations do you want?: ");
    bool v = float.TryParse(Console.ReadLine(), out float maxDifficulty);

    if (!v || maxDifficulty < 10) {
        Console.WriteLine("Write valid number.");
        continue;
    }
    MainWork(path, maxDifficulty);
	Console.Write("Do you want to exit? ( y / n ):");
	path = Console.ReadLine();
	if (path == "y" || path == "Y") break;
	else Console.Clear();
}

partial class Program {
    [GeneratedRegex(@"^[A-Za-z]:(?:\\[^\\\/:*?""<>\|]+)*\\[^\\\/:*?""<>\|]+(?:\.\w+)?$")]
    private static partial Regex pathRegex();
}