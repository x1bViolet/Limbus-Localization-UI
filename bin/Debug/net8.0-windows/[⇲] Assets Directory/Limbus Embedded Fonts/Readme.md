# Built-in game fonts
This is a set of fonts used in the game. They can be used in text as follows:

```ini
<font="BebasKai SDF"> (Enemy names in battle)
<font="ExcelsiorSans SDF"> (Skills Base/Coin power text)
<font="EN/cur)Caveat-SemiBold/Caveat-SemiBold SDF"> (Cathy notes EN)

(English Titie/Context)
<font="EN/title)mikodacs/Mikodacs SDF">
<font="EN/Pretendard/Pretendard-Regular SDF">

(Korean Titie/Context)
<font="KR/title)KOTRA_BOLD/KOTRA_BOLD SDF">
<font="KR/p)SCDream(light)/SCDream5 SDF">

(Japanese Titie/Context)
<font="JP/title)corporate logo(bold)/Corporate-Logo-Bold-ver2 SDF">
<font="JP/HigashiOme/HigashiOme-Gothic-C-1">
```

Limbus supports only these font assets within `<font>` tags (At least these are the ones I know of and that are often seen in the game; there may be more).
These files are also used in custom language properties of the original game translations.




# CompositeFont Definitions.json file

This file defines which Unicode regions should be forced to be occupied by one of these fonts.

In the game, this clearly applies to Japanese and Korean characters in texts that displayed with Title font: they are displayed strictly in Corporate Logo Bold and KOTRA BOLD respectively. So far (March 15, 2026), they haven't done anything about this and likely won't for a very long time.

---

The file is divided into two sections: `"Title"` and `"Context"` (Context remains unchanged for now).

> [!NOTE]
> This example json will be updated with new lines as the explanation progresses, they will be marked with the `/*-----*/` comment.

```jsonc
{
  "Title": [ ... ],
  
  "Context": [ ... ]
}
```

---

In each section you can add a key of the following type:

```jsonc
{
  /* ------------------------------------------------*/
  "Title": [
    {
      "Font": "Font file path or just family name",
      "Unicode": [
        // Unicode ranges list there (Explained below)
      ]
    }
  ],
  /* ------------------------------------------------*/

  "Context": [ ... ]
}
```

The lines inside the `"Unicode"` list must look like this: `"<Unicode start point>-<Unicode end point>"` (Without prefixes like `\u` or `\x`, just something like `"4E00-9FCB"`)

```jsonc
{
  "Title": [
    {
      "Font": "Font file path or just family name",
      "Unicode": [
        /* -------*/
        "0020-007E",
        "00A0-00BF"
        /* -------*/
      ]
    }
  ],

  "Context": [ ... ]
}
```