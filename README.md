SDR# Plugin Manager
=====================

This is a simple program which helps adding and removing plugins to SDR# so you don't have to edit SDRSharp.Config by hand.

## Main functions are still under development
I'm going to create a release as soon as there is anything to release

## Tested plugins
This application should add any valid SDR# plugin to the config file, however it can fail when something unexcepted happens, like some undetectable (*to be precise: I don't know how to detect yet*) file is also required to run your plugin. So here is a short list of successfully added plugins so far:

### Standard plugins
- Digital Noise Reduction
- Frequency Manager
- Noise Blanker
- Recording
- Zoom FFT

### [Jeff Knapp's plugins](http://www.sdrsharpplugins.com/)
- Scanner Metrics
- Frequency Entry
- Frequency Manager + Scanner

Although these plugins come in 3 separate DLLs, they cooperate, so you must add all of them to get them work properly. `Scanner Metrics` requires `System.Data.SQLite` to run. It is also included in the zip just don't forget to copy along with the plugins.

### Other plugins
- [Level Meter](http://levelmeter.sub-web.de/)

I'm not planning to create and maintain a plugin list because there are two great one already. Check http://sdrsharp.com/#plugins or http://www.rtl-sdr.com/sdrsharp-plugins
