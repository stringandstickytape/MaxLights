# MaxLights
A Windows app to control Lifx, WLED and RGB.Net (Corsair/MSI/Logitech/ASUS currently supported) LEDs.

# Overview
MaxLights uses drag-and-drop functions, which combine to create effects.  The following diagram creates a chasing-rainbow effect by varying the hue across 258 LEDs, while setting the brightness and saturation to maximum (65535):

![2](https://user-images.githubusercontent.com/4246218/149525931-86f678b4-549e-455c-84eb-b267f80446af.png)

# Features
* Send lighting effects to LIFX, WLED and RGB.Net (Corsair/MSI/Logitech/ASUS) devices
* Drag-and-drop functions which combine to create effects
* Audio, Screen, Keyboard reactivity
* More functions than you can shake a weirdly antialiased stick at

# Installation
1) Download the latest release from https://github.com/stringandstickytape/MaxLights/releases .
2) Unzip it to the folder of your choice.
3) Run MaxLights.exe
4) If you see a Windows firewall popup, allow MaxLights appropriately

# Desktop Window
On startup, MaxLights presents the following window:

<img width="595" alt="1" src="https://user-images.githubusercontent.com/4246218/149524582-9d84b198-97f7-47bc-8ab6-57cf104b1dd7.png">

Any LIFX devices detected on your LAN will be shown automatically.  If "Enable PC Hardware Support" is ticked, there will be a ten-second delay on startup, and any detected devices will be listed.  WLED devices must be added manually (see below).

# Web UI
Diagrams for effects are created through a web UI, to allow for easy visual programming.  Click the "Launch Web UI" button to launch the UI.

If no diagram is currently loaded, the Web UI will look something like this:

![3](https://user-images.githubusercontent.com/4246218/149527371-5b0a5ffb-1fcd-4de6-9bfa-d2a638eef06e.png)

To create a basic effect:

1) Right-click some whitespace to pop the functions menu (note the search box at the top):

<img width="338" alt="4" src="https://user-images.githubusercontent.com/4246218/149527635-36ec704c-f2d2-4772-b3e6-e368a50bf482.png">




# Readme TODO:
* Adding WLED devices manually

