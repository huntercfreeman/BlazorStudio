# TryingDifferentApproach

This folder exists because there is a fundamental issue with how I wrote the PlainTextEditor.

When one works with extremely large files it is imperative that one:
- Does not read the entire file into memory.
- Does not create a copy of the file that the PlainTextEditor is modifying.

The previous two bullet points present a few questions:
> Does not read the entire file into memory
- How does one continually through random memory access build and destroy small 'chunks' of the file into tiny individual PlainTextEditor instances.
> Does not create a copy of the file that the PlainTextEditor is modifying.
- How does one maintain the state of modifications made to the file?

There is an important concept when developing the next iteration of the PlainTextEditor that must be remembered through development.
- If the user makes an edit to line 1 of the file the rest of the document is 'unchanged' in a sense.
  - That is to say, the content of every line after line 1 was not changed.
  - The byte position of the lines that follow line 1 however, may have been changed.
    - If the user hits the 'Backspace' key, they have effectively shifted all bytes that follow by 1 byte (or whatever the character byte size is).
    - As such one only should maintain changes to the document and all the byte shifts that have occurred. This minimized the state that must be maintained.
    - When the user saves the file then any changes can be written out and the list of changes can be cleared.
