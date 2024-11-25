# ILV Album Optimizer
### ⚠️This software or its developers will never ask for your seed phrase or ask you to sign any transactions!⚠️
### ILV Album Optimizer is a windoews application that helps with sleeving and unsleeving you illuvium beyond album with a few clicks!

## Options
- **Dry Run** - simulate execution. 🔴**TURN THIS OFF**🔴 if you want the app to do changes to your album.
- Print only optimizations - limit the output log to only display optimizations done.

## **[Tested With]** Usage with Firefox

- Open Firefox.
- Go to [beyond].
  - If you're not logged in, do so.
  - Open Developer tools: F12
  - Open Netwrork Tab
  - Refresh the page
  - Look for https://api.illuvium-game.io/gamedata/illuvitars/album/collections/ api call.
  - Look for the request headers **IN RAW VIEW**
  - Copy the whole thing and paste it in the token field.

## Usage with Chrome

- Open Chrome.
- Go to [beyond].
  - If you're not logged in, do so.
  - Open Developer tools: CTRL + SHIFT + I
  - Open Application tab
  - Open Local Storage
    - **[Recomended]** Copy the whole value for 'persist:auth' key.
    - Copy the token only.
	
![Alt text](Tutorial/GetTokenFromChromeTutorial1.png)

## Tech

ILV Album Optimizer uses a number of open source projects to work properly:

- [ModernWPFUI] - Front end.
- [Imx.Sdk] - Reading assets from immutable x blockchain.

## Development

##### Want to contribute? Great!
##### Want to support me? Use my [affiliate link when you buy more disks](http://link.illuvium.io/Ned).
##### Want to gift something on-chain? 0xNed.crypto (0x40e816b38af1e2cc60859bc7f9be01f3ce78c3c0)

## License

MIT

**Free Software, Hell Yeah!**

[//]: # (References)
   [beyond]: <https://beyond.illuvium.io/album>
   [Affiliate]: <http://link.illuvium.io/Ned>
