# WorkspaceSwitch.NET

This is a Windows tool which allows to bind hotkeys for

- switching virtual desktops
- move a window to a specific virtual desktop
- move a window to a specific virtual desktop and switch to the same desktop
- (un-)pin windows so they appear on all virtual desktops

for up to 10 virtual desktops.

It also shows the current desktop number in the tray icon, again, for the first 10 virtual desktops.

## Why?

Markus Scholtes's [VirtualDesktop](https://github.com/MScholtes/VirtualDesktop) and [PSVirtualDesktop](https://github.com/MScholtes/PSVirtualDesktop) are very useful when using Virtual Desktop functionality on Windows.

Before saying anything else, I want to thank Markus because he keeps chasing this moving target. Windows does not offer a stable API for virtual desktops. There are com classes which can be used to programmatically access Windows's Virtual Desktops, but the IDs change all the time. This project is 5% of my work and 95% of Markus's Work in VirtualDesktop. 

Personally, I like to switch virtual desktops with hotkeys, because that's what I'm used to coming from UN*X desktop environments. While I could use tools like [ahk](https://www.autohotkey.com/) to bind the above tools to hotkeys, depending on the machine, it can be somewhat slow to launch a .NET executable per key press. I do like .NET though, so rather than writing an unmanaged native executable which could indeed be faster, I thought I could have a background .NET program listening for global hotkeys. I'm all about quick wins. I could achieve this faster than the former. ;)

As soon as Windows ships with hotkeys that allow to address specific virtual desktops, or stabilises the API so this can be included with [PowerToys](https://github.com/microsoft/PowerToys), I'm happy to close this project and go home. 

## Usage

WorkspaceSwitch.NET registers global hotkeys `MODIFIER+1..0`:
- Switch Virtual Desktop
  - Default `WIN+1..0`
  - modifiers can be changed with command line parameter `--switch-desktop-modifiers`
- Move active window to target virtual desktop
  - Default `WIN,SHIFT+1..0`
  - modifiers can be changed with command line parameter `--move-window-modifiers`
- Move active window to target virtual desktop and switch to the same virtual desktop
  - Default `CTRL,WIN,SHIFT+1..0`
  - modifiers can be changed with command line parameter `--switch-and-move-modifiers`

Additionally, pinning/unpinning windows to all desktop ("sticky") is possible with the default key combination `CTRL,WIN+T`. It can be overridden with the parameter `--sticky-key-combination`, followed by `MODIFIER1,MODIFIER2,...+KEY`

Please note that the default binding `WIN+1..0` are already bound to the taskbar. To disable these, you can use the following registry command:

```
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "DisabledHotkeys" /t REG_SZ /d "1234567890" /f
```

## Build

Use Visual Studio. I have not updated/tested the VSCode task examples recently.

The subproject VirtualDesktop is built depending on Windows version, because the API is not stable. At this point, this has to be configured manually.

Copy LocalSettings.csproj.template to LocalSettings.csproj and adjust the exe file reference according to your Windows version.

## Anticipated Questions

### Why are there no binary releases?

Basically, because there needs to be executables per Windows version due to the unstable com classes.

This could indeed be done in a similar fashion as Markus provides several executables for individual Windows versions. I could use specific buiild configurations, and I suppose building the targets could even be automated via Github. But so far, this hasn't been a priority.

### Why is this not available via Winget?

I love winget. Because there are no binary releases yet. ;) I don't even know if there's a practical way with winget to target different Windows versions.

### Why is this still on .NET framework?

Some of the machines I use this on do not allow me to place random executables on them and do not support .NET as a build target yet. As soon as this changes, I'll look into using .NET.

### Why don't you use Autohotkey?

I like AHK and I have used it for this purpose in the past, until I ran into machines that don't just allow me to install random executables, even worse, some organisations might even frown upon the tool itself.

But it's certainly possible to use something like [VirtualDesktopAccessor](https://github.com/Ciantic/VirtualDesktopAccessor) from AHK.

### Why don't you use Rust?

Yeah, why don't I? I like Rust. Binding global Windows hotkeys with Rust is possible, but it would not be possible to reuse the work in VirtualDesktop. Calling .NET API from Rust is theoretically possible, but I'm not aware of anything that went beyond a PoC. Even if it were usable, this approach would kinda defeat the purpose.  

Building on [VirtualDesktopAccessor](https://github.com/Ciantic/VirtualDesktopAccessor) might be a possibility for the future.

That said, my commercial activities revolve around .NET. The likelyhood of me finding a Visual Studio and a .NET SDK is a lot higher than finding rustc or cargo.

