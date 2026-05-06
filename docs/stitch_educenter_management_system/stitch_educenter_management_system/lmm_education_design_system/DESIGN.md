---
name: LMM Education Design System
colors:
  surface: '#faf8ff'
  surface-dim: '#d9d9e4'
  surface-bright: '#faf8ff'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f3f3fe'
  surface-container: '#ededf8'
  surface-container-high: '#e7e7f3'
  surface-container-highest: '#e2e1ed'
  on-surface: '#191b23'
  on-surface-variant: '#434654'
  inverse-surface: '#2e3039'
  inverse-on-surface: '#f0f0fb'
  outline: '#737686'
  outline-variant: '#c3c5d7'
  surface-tint: '#1353d8'
  primary: '#003fb1'
  on-primary: '#ffffff'
  primary-container: '#1a56db'
  on-primary-container: '#d4dcff'
  inverse-primary: '#b5c4ff'
  secondary: '#006a61'
  on-secondary: '#ffffff'
  secondary-container: '#86f2e4'
  on-secondary-container: '#006f66'
  tertiary: '#852b00'
  on-tertiary: '#ffffff'
  tertiary-container: '#ad3b00'
  on-tertiary-container: '#ffd4c5'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#dbe1ff'
  primary-fixed-dim: '#b5c4ff'
  on-primary-fixed: '#00174d'
  on-primary-fixed-variant: '#003dab'
  secondary-fixed: '#89f5e7'
  secondary-fixed-dim: '#6bd8cb'
  on-secondary-fixed: '#00201d'
  on-secondary-fixed-variant: '#005049'
  tertiary-fixed: '#ffdbcf'
  tertiary-fixed-dim: '#ffb59a'
  on-tertiary-fixed: '#380d00'
  on-tertiary-fixed-variant: '#802a00'
  background: '#faf8ff'
  on-background: '#191b23'
  surface-variant: '#e2e1ed'
typography:
  display-lg:
    fontFamily: Inter
    fontSize: 3rem
    fontWeight: '700'
    lineHeight: '1.2'
    letterSpacing: -0.02em
  display-md:
    fontFamily: Inter
    fontSize: 2.25rem
    fontWeight: '700'
    lineHeight: '1.2'
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Inter
    fontSize: 1.875rem
    fontWeight: '600'
    lineHeight: '1.3'
  headline-md:
    fontFamily: Inter
    fontSize: 1.5rem
    fontWeight: '600'
    lineHeight: '1.3'
  headline-sm:
    fontFamily: Inter
    fontSize: 1.25rem
    fontWeight: '600'
    lineHeight: '1.4'
  body-lg:
    fontFamily: Inter
    fontSize: 1.125rem
    fontWeight: '400'
    lineHeight: '1.75'
  body-md:
    fontFamily: Inter
    fontSize: 1rem
    fontWeight: '400'
    lineHeight: '1.5'
  body-sm:
    fontFamily: Inter
    fontSize: 0.875rem
    fontWeight: '400'
    lineHeight: '1.5'
  label-md:
    fontFamily: Inter
    fontSize: 0.875rem
    fontWeight: '500'
    lineHeight: '1.25'
    letterSpacing: 0.05em
  label-sm:
    fontFamily: Inter
    fontSize: 0.75rem
    fontWeight: '600'
    lineHeight: '1'
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  unit: 4px
  space-1: 0.25rem
  space-2: 0.5rem
  space-3: 0.75rem
  space-4: 1rem
  space-6: 1.5rem
  space-8: 2rem
  space-12: 3rem
  container-max: 1440px
  gutter: 1.5rem
  margin-page: 2rem
---

## Brand & Style

The design system is anchored in the **Corporate / Modern** aesthetic, specifically tailored for the high-stakes environment of educational management. The visual language communicates reliability, institutional stability, and modern efficiency. 

The strategy prioritizes a "SaaS-like" clarity, utilizing high-functionality layouts that minimize cognitive load for administrators, teachers, and students. The emotional response should be one of "effortless organization"—where the interface feels like a sophisticated tool rather than a complex hurdle. The style leverages a refined balance of generous whitespace, a systematic grid, and subtle depth to guide the user's focus toward critical educational data.

## Colors

The palette is dominated by **Professional Blue (#1A56DB)**, establishing an immediate sense of trust and authority. To provide functional contrast, a **Vibrant Teal (#0D9488)** serves as the primary action color for positive affirmations and secondary navigations, while **Soft Orange (#F97316)** is reserved for high-priority calls to action and critical attention points.

Role-based indicators are strictly enforced to provide instant context:
- **Staff/Admin:** Uses the primary Blue, signifying the core infrastructure.
- **Teacher:** Uses a distinct Emerald Green to represent growth and guidance.
- **Student:** Uses Indigo to differentiate from administrative functions while maintaining a scholarly feel.

The background utilizes a cool neutral gray scale to maintain a clean, "SaaS" appearance, ensuring the colorful role indicators and CTA buttons remain highly legible.

## Typography

This design system utilizes **Inter** exclusively to achieve a systematic, utilitarian aesthetic. The typeface's tall x-height and excellent legibility make it ideal for data-heavy management screens.

The hierarchy is built on a clear distinction between "Display" styles for dashboards and "Body" styles for intensive reading. Headlines use a tighter letter-spacing and heavier weights to command attention, while body text uses a generous 1.5x to 1.75x line height to ensure readability during long grading or administrative sessions. Labels are occasionally set in uppercase with slight tracking to differentiate metadata from interactive content.

## Layout & Spacing

The layout follows a **Fixed-Fluid Hybrid Grid**. Main application dashboards use a 12-column fluid grid to maximize the utility of varying screen sizes, while content-heavy pages (like lesson builders or reports) are constrained to a 1440px max-width container to maintain line-length integrity.

Spacing is built on a 4px baseline shift, but the design leans toward a **generous whitespace** philosophy. Margins between major sections should never be less than `space-8` (32px), creating "breathing room" that prevents the software from feeling cluttered or overwhelming. Component internal padding should prioritize clarity, using a minimum of `space-4` (16px) for standard touch targets.

## Elevation & Depth

Visual hierarchy is established through **Tonal Layers** combined with **Ambient Shadows**. The design avoids harsh outlines in favor of soft depth cues:

1.  **Background (Level 0):** The base canvas uses the `neutral-base` hex (#F9FAFB).
2.  **Surface (Level 1):** Cards and main content containers are pure white (#FFFFFF). They use a very soft, diffused shadow (Offset: 0, 4px; Blur: 6px; Opacity: 0.05) to separate from the background.
3.  **Raised (Level 2):** Hover states and active dropdowns use a more pronounced shadow (Offset: 0, 10px; Blur: 15px; Opacity: 0.1) to indicate interactivity and "closeness" to the user.
4.  **Overlay (Level 3):** Modals and notifications use high-contrast depth to dim the background and focus all attention on the task at hand.

This approach creates a clean, stacked feel that is hallmarks of modern SaaS interfaces.

## Shapes

The design system employs a **Rounded** shape language to soften the professional tone, making the platform feel accessible and user-friendly. 

- **Standard Elements (Buttons, Inputs):** 8px (0.5rem) corner radius.
- **Large Containers (Cards, Modals):** 16px (1rem) corner radius.
- **Extra-Large Elements (Hero sections, Profile Banners):** 24px (1.5rem) corner radius.

Consistent rounding across all interactive elements ensures that the UI feels like a single, cohesive ecosystem.

## Components

- **Buttons:** Primary buttons use the deep blue with white text. CTA buttons for "Add Student" or "Create Course" utilize the vibrant teal. All buttons feature a 300ms transition on hover, deepening the background color slightly.
- **Role Chips:** Small, pill-shaped badges (rounded-full) with low-opacity backgrounds and high-contrast text. For example, a "Teacher" badge uses a light green background with dark green text.
- **Input Fields:** Use a subtle 1px border (#E5E7EB) that transitions to the primary blue on focus. Error states swap the border to red with a 4px outer glow.
- **Cards:** The primary container for data. They must always include a consistent header section and a 16px padding.
- **Lists:** Clean, borderless rows with a subtle divider line (#F3F4F6). On hover, rows should highlight with a light gray tint to improve scanability.
- **Progress Bars:** Essential for student tracking. These should use the student-role indigo for individual progress and the primary blue for institutional progress.
- **Data Tables:** High-density layouts with sticky headers and distinct alternating row colors for complex management tasks.