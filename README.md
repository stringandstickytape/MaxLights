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
- developers can add functions simply by adding a new class, which will be discovered at runtime via reflection.  Add.cs is a good example of a simple such function.

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

2) Add a Number-To-List function.  Set its Item to 65535 (the maximum value for hue, saturation or brightness), and set its Items to Generate to 1.  

3) Connect the Number-To-List output "List of Numbers" to the Hue, Saturation and Brightness inputs of your chosen device.  

4) Connect your chosen device's Rendered Light output to the Renderer.  THIS IS CRITICAL - lights not connected to the renderer will not render (!), and indeed will not even be uploaded from client to server.

Your diagram should now look something like this:

![5](https://user-images.githubusercontent.com/4246218/149528185-f04fff7a-12a3-41cd-999a-d8cda0ec8c84.png)

5) Now upload the diagram.  The first LED on the device you selected, should turn red, because we have set hue to 65535 (red), and saturation and brightness to 65535 (maximum).

# Examples

* Sound responsive

The following diagram will make the first ten LEDs on a device, react to a range of frequencies in the audio output.  Auto Gain is used to ensure a full range of hues.  You could also use one of the expand functions to make this into ten colour areas across a larger range of LEDs.

![6](https://user-images.githubusercontent.com/4246218/149528999-5a2079ec-0e77-4ffe-98c6-a78865e7bc43.png)

The following diagram will set the first 100 LEDs on a device to a random colour, on every beat of the currently-playing music:

![7](https://user-images.githubusercontent.com/4246218/149529717-f18de62a-4e24-4a7d-b62a-f036b6ef302c.png)

* Screen Responsive ("Ambilight")

The following diagram will take 300 screen samples in a line down the left-hand-side of monitor 0, and send them to the chosen device.  Values were calculated by using the 3840x2160 helper button - but you must set Monitor manually.  Note the use of "HSB" as an alternative to separate H/S/B connections:

![8](https://user-images.githubusercontent.com/4246218/149530248-86e7ed4e-ec87-4346-980f-586992d79fb8.png)

The following diagram will take ten screen samples from *each* side of monitor 0, combine them into a list of twenty samples, shuffle that list, then send the first ten to one device and the remainder to another device (!) :

<img width="1005" alt="9" src="https://user-images.githubusercontent.com/4246218/149531414-27cdfcd6-26fd-4f46-9d40-3d260dacf328.png">

* Keyboard response

The following diagram will set the first three LEDs of the selected device, to red.  When a key is pressed, the colour will change to a hue based on the code of the key pressed:

<img width="809" alt="10" src="https://user-images.githubusercontent.com/4246218/149535297-395ab799-b5eb-4fb5-b495-8df22cf0ed3d.png">

# Readme TODO:
* Adding WLED devices manually
* Acknowledgements for the various genuises upon whose work, MaxLights is built

