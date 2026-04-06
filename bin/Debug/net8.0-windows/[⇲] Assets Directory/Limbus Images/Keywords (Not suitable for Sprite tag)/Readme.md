These files are needed to override the sprite used for [KeywordID] keyword link expressions in the text, as well as in the keyword editor itself or in the pop-up information when displaying icons near the name.

Structure of json file to create icon redirection:
```jsonc
{
  "ExistingKeywordImageID": {
    "SomeOtherID1",
    "SomeOtherID2"
    // `SomeOtherID1` and `SomeOtherID2` will use `ExistingKeywordImageID` sprite when inserted through [KeywordID] in text or shown in the editor / pop-up info
  }
}
```



Plus there is no way to tell which keywords have their own sprites and which don't, unless print them all in a `<sprite id="...">` tags and check manually in-game.

<details>

<summary>Printing script example</summary>

Printing example using LC Localization Interface code infrastructure (`LimbusLocalizationFile<T>` and `PlainKeyword` classes + (extension) `FileInfo.DeserealizeJsonAs<T>`):
```cs
FileInfo[] KeywordFiles =
[
    new(@"C:\Program Files (x86)\Steam\steamapps\common\Limbus Company\LimbusCompany_Data\Assets\Resources_moved\Localize\en\EN_BattleKeywords-a1c9p1.json"),
    new(@"C:\Program Files (x86)\Steam\steamapps\common\Limbus Company\LimbusCompany_Data\Assets\Resources_moved\Localize\en\EN_BattleKeywords-a1c9p2.json"),
    new(@"C:\Program Files (x86)\Steam\steamapps\common\Limbus Company\LimbusCompany_Data\Assets\Resources_moved\Localize\en\EN_BattleKeywords-a1c9p3.json")
];
string Total = "";
foreach (FileInfo KeywordsFile in KeywordFiles)
{
    foreach (PlainKeyword Keyword in KeywordsFile.DeserealizeJsonAs<LimbusLocalizationFile<PlainKeyword>>()!.DataList)
    {
        Total += $"<sprite name=\\\"{Keyword.ID}\\\">\\n";
    }
}
Clipboard.SetText(Total); // 'Clipboard' available only in WPF or WinForms
```
</details>