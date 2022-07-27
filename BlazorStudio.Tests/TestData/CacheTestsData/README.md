# Font dimensions are monospaced and:
// TODO: The font-size style attribute does not equal the size of the div that encapsulates the singular character. Figure out EXACTLY these values based off the font-size instead of hard coding what developer tools says
protected double HeightOfEachRowInPixels = 27;
protected double WidthOfEachCharacterInPixels = 9.91;

# Test cases are read as a matrix position relative to the corresponding .png that contains images of the situation.
Example: '0,0_partial.txt' maps to 'first row, first column_in the PartiallyOverlappedChunks.png image'
Example: '2,0_partial.txt' maps to 'third row, first column_in the PartiallyOverlappedChunks.png image'
Example: '2,1_partial.txt' maps to 'third row, second column_in the PartiallyOverlappedChunks.png image'

# Colorscheme
The black rectangle is the cached chunk
The blue rectangle is the request