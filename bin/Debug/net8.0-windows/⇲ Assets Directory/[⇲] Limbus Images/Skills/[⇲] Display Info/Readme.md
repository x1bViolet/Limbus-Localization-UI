# Raw Json
The folder is intended for raw .json files of the game, so as not to convert them into another type of .json structures as it done with E.G.O Gifts, you can drop new files there after updates.


Those files can be found at organized assts drive, corresponding files contain lowercase "skills" in their name.:
https://drive.google.com/drive/folders/1RiPedCxRDQXRAy3koLw75kMRy_ebSfpB






# Constructor
Here you can fill out readable .json files.

Example files avalible at `⇲ Assets Directory\[⇲] Limbus Images\Skills\[⇲] Display Info\Constructor\Examples`

Format of list item with display info (Cinq Assoc. East Section 3 Don Quixote Skill 1):
```json
 {
  "(Name)": "Skill 1  — 「Fa Jin」",

  "ID": 1031101,

  "Specific": {
    "Damage Type": "Pierce",
    "Action": "Attack",
    
    "Rank": 1
  },

  "Characteristics": {
    "Coins List": ["Regular", "Regular"],
    "Coins Type": "Plus",

    "Attack Weight": 1,
    "Level Correction": 2
  },

  "Upties": {
    "1": {
      "Coin Power": 3,
      "Base Power": 3,
      "Affinity": "Gluttony"
    },
    "3": {
      "Coin Power": 4
    }
  }
}
```