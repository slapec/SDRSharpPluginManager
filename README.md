SDR# Plugin Manager
=====================

This is a simple program which helps adding and removing plugins to SDR# so you don't have to edit SDRSharp.Config by hand.

## How to use it?
1. Download the [latest release](https://github.com/slapec/SDRSharpPluginManager/releases)
2. Put `SDRSharpPluginManager.exe` into SDR#'s folder or run it where it is and follow the instructions
3. Add a plugin DLL, remove unused ones and save changes
4. Enjoy!

## Working plugins
The application should add any valid SDR# plugin to the config file, however it can fail when something unexcepted happens, like some undetectable file is also required to run your plugin (*to be precise: I don't know how to detect yet*). So here is a short list of successfully added plugins so far:

### Standard plugins
- Digital Noise Reduction
- Frequency Manager
- Noise Blanker
- Recording
- Zoom FFT

### [Jeff Knapp's plugins](http://www.sdrsharpplugins.com/)
- Frequency Entry
- Frequency Manager + Scanner
- Scanner Metrics

Note: Although these plugins come in 3 separate DLLs, they cooperate, so you must add all of them to get them work properly. `Scanner Metrics` requires `System.Data.SQLite` to run. It is also included in the zip just don't forget to copy along with the plugins.

### [Vasili's plugins](http://www.rtl-sdr.ru/category/plugin)
- Audio Recorder
- Baseband Recorder
- CTCSS Decoder
- DSD Interface
- Digital Audio Processor
- Frequency Manager
- Frequency Scanner
- Time Shift

Note: `Frequency Manager` is a drop-in replacement of the one which comes with SDR#. You can't use both at the same time.

### Other plugins
- Satellite Tracker
- [DDE Tracking Client](http://www.satsignal.eu/software/DDETracker.html)
- [EasyScanner](http://easyscanner.sub-web.de/)
- [Level Meter](http://levelmeter.sub-web.de/)
- [Trunker](http://forums.radioreference.com/software-defined-radio/265660-sdr-trunking-has-been-updated-6.html#post2155643)


I've collected these plugins from http://sdrsharp.com/#plugins and http://www.rtl-sdr.com/sdrsharp-plugins. Feel free to message me about other working plugins or just fork this repo and add new ones by your own.

## Not working plugins
There aren't any *yet* \o/

## Upcomming features
- [ ] Copy the added DLL to SDR# folder
- [ ] Enable/Disable feature
- [ ] Reordering
- [ ] More detailed error messages in nice dialogs

## Compatibility
As I was testing plugins I saw that some of them are broken due to SDR# API changes. This application loads all dependent files from where the manager EXE is located or where it is pointed to on the startup. So as long as the `ISharpPlugin.DisplayName` property is in the plugin and the config file structure is not changed it should load everyting.
The oldest SDR# revision I found was 1189 and there were no issues.
