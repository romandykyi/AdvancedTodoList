using AdvancedTodoList.Core.Mapping;
using Mapster;

namespace AdvancedTodoList.UnitTests.Mapping;

[TestFixture]
public class InputSanitizingTests
{
	private class Poco
	{
		public string? Text { get; set; }
		public DateTime Date { get; set; }
		public DateTime? NullableDate { get; set; }
	}

	private record TextDto(string Text);
	private record DateDto(DateTime Date, DateTime? NullableDate);

	[SetUp]
	public void SetUp()
	{
		MappingGlobalSettings.Apply();
	}

	[Test]
	public void NullString_MapsIntoEmptyString()
	{
		// Arrange
		Poco poco = new()
		{
			Text = null
		};

		// Act
		var dto = poco.Adapt<TextDto>();

		// Assert
		Assert.That(dto.Text, Is.EqualTo(string.Empty));
	}

	[Test]
	public void String_MapsIntoTrimmedString()
	{
		// Arrange
		string expectedText = "Lorem ipsum dolor sit amet, ...";
		Poco poco = new()
		{
			Text = $"\t\r  {expectedText} \t\t\r   "
		};

		// Act
		var dto = poco.Adapt<TextDto>();

		// Assert
		Assert.That(dto.Text, Is.EqualTo(expectedText));
	}
}
