README
----

For information on file formats, please see FORMATS.txt

KEY NOTES
----
This is still very alpha code - as such don't trust it too much, and keep regular updates of the relevant XML files used by the app. This software comes with no warranty, and no blame is accepted for any damages which arise from its use.


Usage
----
The app is based around 4 tab pages:-
1) Spells: Allows you to save lists of spells in DocX format, print them out, and save/load sets of them as Spellbooks.
2) Encounters: Allows you to create lists of Monsters, save them in DocX, print them, and save/load sets as Encounters.
3) Paper Mini Maker: Allows creation of paper minis for monsters. Creates as PDF.
4) Settings: Pick XML files for each of the data categories, and import new spells and monsters, plus Edit existing ones.


SPELLS
----
To use, first import the All Spells.xls file via the Settings tab. 
The checklist boxes on the left can be used to filter spells you want. To check/uncheck all the entries in "By Name" use the checkboxes at the bottom. (TODO: Buttons may make more sense). Check any spells "By Name" that you want.
Once you've checked the spells in "By Name" that you care about, click "Add Checked". This will add them to the main list. "To DocX" saves this in a DocX file.
To save this as a spell book, type a spell book name (e.g. the name of a character) in the top-right combobox, then click "Save". To load a spellbook select it from the pulldown and click "Load". "Del" deletes the currently selected Spellbook - to reset the current list click on "Reset" instead.


ENCOUNTERS
----
To use, first import the "BR Monsters.xls" file via the Settings tab, under Encounters.
Manual Entry:
 To add new monsters to an encounter, select the monster in the pulldown under "Create Encounter" and click "Add". If you just want a line with the name and HP, check the "Summary Only" checkbox first.
Auto Entry:
 Click on "Encounter Generator" to automagically create an encounter, tacking into account the multipliers for multiple monsters. Then, in the new popup:-
 1) Set the party settings in the "Party" section. This will generate a target XP threshold for monsters. The Current XP shows the total XP for all currently added monsters
 2) Pick how many monsters you want to get, and the total XP to aim for - this is set to the threshold in (1) by default.
 3) Pick the monster group type "Max group" creates lots of low XP baddies, whereas "Max single" aims for a biggie. "Boss at x%" aims for a single with x% of the XP, and the rest made up of others. No intelligence is used to pick monsters, so you can end up with a Lich and a load of Hawks, or other illogical setups.
 4) In the treeview on the left, uncheck things you don't want. Note, this works by _unchecking_ not checking. It sucks, and I'm in the process of changing how this works to be more like how spell selection works. In the "By name" branch, as you uncheck things they will uncheck here - if you want them, recheck them. Anything with an XP greater than the threshold is greyed out, but can be checked.
 5) When you're happy', click "Roll". The selected monsters will appear at the bottom. Click "Roll" to come up with a different set. Uncheck any you don't want added, then hit "Add Selected". This will update the list on the main page with those added, and update Current XP, and hence the Remaining XP threshold and the Target XP in step (2). Repeat as appropriate.
 6) Click the close icon at the top-right of the dialog window to close it when happy.
On the main page, next to each monster are two boxes. The empty one can be used to give a name e.g. "Bob". The other shows the auto-generated HP - this can be changed if desired.
Once you're happy, you can Save just this set of monsters to a DocX with the "To DocX" button.
You can also save this as an encounter, just type an encounter name (I use the "map room#" e.g. "Cragmaw 2") then click "Save". "Load" and "Del" are obvious.
To export multiple encounters (e.g. all those for a given map), use "Batch To DocX". This will pop up a dialog which lists the "Current" and all saved encounters. Check those you want, then click on "Save".
Finally Print/Print Preview work, but they aren't pretty at the moment - I recommend using the DocX route, and then playing with font sizes etc if you want.

PAPER MINI MAKER
---
This relies on you having set images for any creatures you want minis of via the SETTINGS tab, EDIT MONSTERS button. See there for details. Without images, it still works, but will only show the monster name and number.
The settings on the left allow you to configure the size of the minis:-
- Width: Width of a medium monster - i.e. 5' wide. Large is double this, and so on.
- Blank footer height: How much blank space at the bottom, when folded, to insert into a holder.
- Text font size: The default Sans font is used.
- Max height (medium): When folded in half, what is the max height you want.
- Max height: Larger the Medium monsters can be larger. The height is scaled up to try to maintain the aspect ratio of the source image file. This value is the absolute max height, irrespective of width.
- Padding: Space between the border and image, and image and text.
- Border width: How large should the black border be. This is mainly just used as a guide for cutting.
- Border size: Border size on the paper. Printers can't do 100% of the width/height of the paper, you may need to raise this if your printouts look iffy.
- Paper type: If you print with the wrong paper, the printer will likely try to scale the page down/up, and so the values above will all be wrong. 
To add a monster a) select it in the pulldown and click "Add", and/or check any encounters you want the monsters for and click "Add Checked". When adding encounters, it will try to only add sufficient for all encounters in parallel. e.g. if Encounter 1 used 2 bugbears, and Encounter 2 used 3 bugbears, it will only add 2 Bugbears as you can reuse the same counters for each.
Once added, the "Count" in each row shows how many of that monster to add. The "Start Index" says what number to start from. e.g.Monster=Bugbear, Count=2, Start Index=5, would create Bugbear #5, Bugbear #6.
When you're happy, click "Save". This will create a PDF and try to arrange the minis in a semi-organized way, trying to minimise wastage. It's not great at that - that's a remarkably hard problem.
Then open the PDF in your normal PDF reader, and print it out. Cut out the tokens by cutting along the outside, and _not_ cutting between the images. You can then fold these in half horizontally, and then stick them into a holder or otherwise fold them.

SETTINGS
----
The "Pick ..." buttons allow you to select an existing file as a source. These files will be automatically saved to. To use a new file, create an empty file with a .xls file extension, then pick it. Your mileage may vary though.
The "Import" buttons can be used to import either XLSX or XML files - see FORMATS.txt for details.

Monsters can be editted with the "Edit Monsters". This should be pretty self-evident. The key thing to note is you have to click "Save" to actually save changes. The "Add" function appears to currently be broken. 
The pulldown boxes at the bottom can be used to filter which monsters you can see - this makes it easier to find things missing specific items.
To add an image, either click "Pick Picture" and then select an image file, or copy the URL of an image and paste it into the box next to "Pick Picture" then click in any other field. There may be a pause while it downloads the file. Once downloaded, it caches the file in your Windows Temp directory, but note that the cache only lastes as long as the program is running. Once you close and reopen the app, any remote files will be downloaded again as needed. For images, I recommend picking those with a white border, cropped as closely as possible. The app will keep the aspect ratio of the image as a whole, so if there's a lot of whitespace then that whitespace will appear on the minis as well.

You can also make some changes via a batch, by clicking "Batch Update" on either the Settings tab or the Edit Monster dialog. Currently only the Environment can be updated this way. To update, check the checkbox of any monsters you want to set an environment of. Then select the relevant environment in the "Value" pulldown, a "!environ" removes a previously set value. Then click "Update" to set these. Click "Exit" when you're done.

Spells can be editted via "Edit Spell". The main thing to note is that the description field is a rich text field, so you can set bold on highlighted text with ctrl+b, italics with ctrl+i, etc. The checkboxes at the bottom can be used to filter only specific spells - those missing a list of materials to be used, and those missing a description.