# Maze Ball Game - Implementation Summary and Design Choices
## Name: Cristina Huang (hh3101)
## Computer platform: MacOS, macOS Sequoia Version 15.3
## Unity version: 2022.3.57f1
## Mobile platform: IOS, 18.3,iPhone 13 promax
## Assets used: 
- Casrtoony Fantasy Lowpoly Crate
- Gems Ultimate Pack
## Challenges and Solutions During Implementation

### 1. **Camera Perspective and Arrow Indicator**
**Challenge:** The arrow meant to guide the player to the endpoint was initially behaving incorrectly due to camera tilt and potential differences in device screen aspect ratio .
**Solution:** Instead of relying on direct screen-space checks, the arrow was positioned in 3D relative to the player and rotated in world space to always point toward the endpoint. 

### 2. **Ball Movement and Tilt Sensitivity**
**Challenge:** The ball was reacting too strongly to extreme tilts, making the game difficult to control.
**Solution:** Implemented a maximum effective tilt angle of 30° so that excessive tilting beyond this threshold would not increase the ball’s movement intensity further.

### 3. **Audio Feedback and Volume Control**
**Challenge:** Audio effects for various interactions needed volume adjustments and smooth transitions.
**Solution:** Added individual volume controls for rolling, jumping, door opening, wall hit, and victory sounds. Implemented gradual start/stop logic for rolling sounds to avoid abrupt audio changes.

### 4. **Game UI Layout and User Experience**
- **Counter and Timer Position:** Originally blocking the game view; moved to the **top-left corner** for visibility.
- **Buttons Placement and Color:** 
  - Buttons placed at the **top-right** for quick access.
  - Buttons are **grey** to discourage players from quitting too easily.
  - Jump button made **transparent** and positioned at the **bottom-right**, since most players use their right hand for tapping.
- **Win Screen Handling:**
  - When the player wins, the **win UI is displayed, and in-game UI is deactivated**.
  - Instead of forcing players back to the start menu (as required by homework), the game now returns to the **Maze Selection screen** for quick level selection.
All buttons are responsive so that the user get feedback when they press the buttons.

### 5. **Level Progression and Saving Data**
**Challenge:** The game needed a way to store level completion status while keeping it simple.
**Solution:** Used **PlayerPrefs** to store level unlocks. Could have used **serialization**, but given the game’s simplicity, PlayerPrefs was sufficient.

### 6. **Button Layout on Victory Screen**
- **Next Level** button: **Right** (matches natural progression).
- **Restart Level** button: **Middle**.
- **Return to Menu** button: **Left**.
This layout is based on common UI design principles and intuition.

### 7. **Visual Clarity and Aesthetic Choices**
- The **ball color** was chosen to distinguish it clearly from the **floor and background**.
- **Two types of collectibles** were introduced:
  - **Diamonds (1 point)**.
  - **Chests (2 points, harder to reach)**.
- **Locked Level Buttons:**
  - When playing for the first time, **Level 2 and Level 3 are disabled** (greyed out and unclickable) until unlocked.
- **Start Scene Background Color:**
  - Set by **changing the camera background color**.

### 8. **Level 3 Maze Using ProBuilder**
- Used **ProBuilder** to create a circular maze by placing walls with pipes of different radius and then deleting surfaces.
- **Holes were created** by detaching and deleting surfaces in ProBuilder.

### 9. **Door and Trigger Color Matching**
- Different **doors and triggers have distinct colors** so that players can easily identify which trigger corresponds to which door.

### 10. **Collectible Animation and Placement**
- **Floating animation** added to collectibles to make them more noticeable.
- Collectibles are **placed along the path** to the endpoint while some (chests) encourage exploration.

### 11. **Rotation Platform Refinement**
- Adjusted rotation platform behavior to **make it more intuitive and reasonable** based on player feedback.

### 12. **EndPoint Visibility**
- The **EndPoint is designed as a rainbow** to make it more visible and distinct.

### 13. **Intuitive UI and Start Button**
- **Large and clear start button** ensures ease of access.

## Implementation of Nielsen's 9 Usability Principles
### 1. **Visibility of System Status**
- Place counter and timer **at the top left corner** to show score and elapsed time in an intuitive and accessible way.
- Arrow indicator ways pointing to the end zone and it rotate around the player to guide the path.
- Jump button is **transparent** so that it does not block the view.

### 2. **Match Between System and Real World**
- Placed UI elements intuitively based on common user habits (e.g., jump button bottom-right for right-hand users).
- Used **intuitive iconography and colors** to convey meaning (e.g., grey buttons indicate locked levels).

### 3. **User Control and Freedom**
- **Restart button** allows players to retry instantly.
- Instead of forcing them to go back to the main menu, players **return to the maze selection screen** after winning. (might be different from the requirement in homework description)

### 4. **Consistency and Standards**
- Used **common game UI conventions**: score at top-left, essential controls at top-right, restart buttons in the center.
- Maintained consistent button placements in menus and in-game UI.

### 5. **Error Prevention**
- Implemented **a maximum tilt angle** to prevent unintended extreme movements.
- Prevented accidental level selection by **disabling locked levels**.

### 6. **Recognition Rather Than Recall**
- Players don’t have to remember which levels are completed—unlocked levels are indicated visually.
- **Clear UI elements** show interaction possibilities at all times.

### 7. **Flexibility and Efficiency of Use**
- **Tap-to-collect feature** makes interactions more efficient.
- **Progression system allows quick access to levels** rather than forcing sequential play.

### 8. **Aesthetic and Minimalist Design**
- Clean and **non-obstructive UI layout**.
- Transparent jump button ensures **unobstructed gameplay visibility**.

### 9. **Help Users Recognize, Diagnose, and Recover from Errors**
- If the player falls off, the **game automatically resets the level** instead of leaving them stuck.
- The level button is grey when it is not unlocked.

