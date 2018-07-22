hdiutil attach -nobrowse ~/Packages/TEST.dmg
cp -R '/Volumes/testapp ExampleAppName/testapp ExampleAppName.app' /Applications
echo 'password' | sudo -S '/Applications/testapp ExampleAppName.app/Contents/MacOS/testapp ExampleAppName' install -keycode=$1 -language=en
diskutil list | grep testapp | diskutil unmount '/Volumnes/testapp ExampleAppName'
