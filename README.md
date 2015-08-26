Dyysh - application for publishing images to the web from PC.
=============================================================

Features
--------
#####Screen capture and image uploading.
Freezes the screen and allows to draw a rectangle for specific area capture. Current capture method involves GDI+. 
Of course, it is also possible to upload images from files and clipboard.
Copies image URL to the clipboard when upload finishes.
#####Connection to the server
Before uploading something, client application needs to be authorized by the main server. This is required to designate an author for every upload. It involves sending encrypted user credentials to server. And then server sends encrypted user GUID, which is added to every upload.
#####Hotkeys
Hotkey support was added thanks to [NHotkey](https://github.com/thomaslevesque/NHotkey).
#####Automatic updates
Compares version of the current running assembly with version described in XML file at the server location. If assembly version is lower than that, new version download will be suggested.

Things to add or change
-----------------------
* Refactor whole app to use MVVM pattern.
* Add simple image editing before uploading.
* Check for updates by consuming WCF service.
* Allow to upload files besides images.
