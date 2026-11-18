using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LearnEnglish.Core
{
	public static class AnswerMatcher
	{
		public static string Normalize(string value) => Regex.Replace(
			(value ?? string.Empty).Normalize(NormalizationForm.FormC).Trim(), @"\s+", " ")
			.TrimEnd('.', '!', '?').TrimEnd().ToLowerInvariant();

		public static bool Matches(string value, string[] answers) =>
			!string.IsNullOrWhiteSpace(value) && answers.Any(x => Normalize(x) == Normalize(value));
	}
}
