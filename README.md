# Limbus Company Localization Interface

## Currently able to edit:
- `Bufs*.json` and `BattleKeywords*.json`
- `Skills*.json`
- `Passives*.json`
- `EGOgift*.json`
- *Custom Identity/E.G.O Preview creation

# [Wiki](https://github.com/x1bViolet/Limbus-Localization-UI/wiki)

## Interface navigation:
1. To save any description, press CTRL + S. For names there are special button.
2. To fast ID switch, click with right mouse button on ID label (LMB is copy), type ID or any Name (Automatically finds an existing one), then press Enter to switch.
3. For jump to end/start of file click with right mouse button to Next/Previous switch buttons.
4. ID switch also can be executed by pressing Left/Right arrows on keyboard or Back/Forward mouse buttons. In skills, you also can hold Shift while pressing Back/Forward mouse buttons to switch between uptie levels.
5. In passives, you can create `"summary"` desc elements for passives by clicking on unhighlighted (Disabled) summary desc switch buttons, tooltip will appear.
6. In skills you can click with right mouse button to coin descriptions on preview for fast switch on them.
7. In `Bufs` you able to edit name in textfield on preview, and 'OK' button saves both name and description.
8. Object name in right menu can be scrolled by drag scroll, same as all limbus previews.

## Files saving behavior
- Program tries to keep original encoding of file on save, otherwise uses UTF8.
- Line break is LF, not CRLF.
- All `null` descs (Non-existing) (Coin descs or regular descs of keywords or skills) being replaced with empty strings.
- **By any of any ways make sure you backup your localization files and after saving file is not corrupted (What should not be).**

------
<img alt="Screenshot 2025-09-04 234750" src="https://github.com/user-attachments/assets/f7eeba5f-39c0-4851-9560-27a7b93ed642" />
<img alt="Screenshot 2025-09-04 234942" src="https://github.com/user-attachments/assets/312ad966-dac6-418a-83c7-3d64a851f0f8" />
<img alt="Screenshot 2025-09-04 235150" src="https://github.com/user-attachments/assets/5909c741-f392-4c00-bfee-892e217533f6" />
<img alt="Screenshot 2025-09-04 235430" src="https://github.com/user-attachments/assets/70ced082-ede8-48b0-b9ca-2e9d4288e5f3" />

