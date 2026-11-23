---
agent: agent
---

Use Playwright MCP tooling to perform the following steps. Don't create code; execute the steps directly using the MCP tooling:

- Navigate to the official DDC 2026 conference page: https://www.developer-world.de/ddc
- Gather the conference's website design (fonts, UI style, colors, and writing style) and content theme. Note its focus on .NET, C#, cloud, AI, and modern software architecture. Save these details for use in the image prompt.
- Gather reference photos of the speakers Harald Binkle and Nico Orschel from Google Image Search (focus on DWX or Infinite AI conferences). Save their speaker profiles, upload the pictures before image generation, and use them as visual references so the generated speakers resemble them.
- Navigate to Google Gemini: https://gemini.google.com/app
- Wait for the user to be logged in.
- Create a prompt for Gemini to generate an image using the conference theme and speaker photos as references. Describe this scene:

"Harald Binkle and Nico Orschel are presenting modern .NET and Playwright tooling on stage at the .NET Developer Conference (DDC) 2026 in Cologne, Germany, on November 23–26, 2026. They wear stylish purple 'Do Epic Shit' T-shirts and sunglasses. The speakers should resemble the uploaded reference photos.

The scene is set in a festive Christmas-season atmosphere at the Hyatt Regency Cologne. Through the venue's windows, show a winter view of Cologne with the Rhine, Hohenzollern Bridge, and Cologne Cathedral recognizable in the distance. Add tasteful Christmas decorations, warm golden fairy lights, a decorated Christmas tree, and subtle seasonal details, while keeping the conference itself professional and focused on technology. Use cozy warm interior lighting contrasted with the cool winter evening outside.

A large, clearly readable banner behind the stage says '.NET Developer Conference DDC 2026 – Köln'. The audience is energetic and engaged, representing a welcoming international developer community. Screens and stage details evoke .NET, C#, cloud, AI, and software architecture without cluttering the scene.

The visual style should harmonize with the DDC website's design and colors, combined with a colorful neo-graffiti / urban mural aesthetic: spray-paint textures, bold contrasts, cinematic lighting, dynamic composition, vivid purples, warm Christmas golds, and cool winter blues. The result should feel festive and visually striking, but still credible as a professional technology conference.

Wide-angle cinematic perspective, realistic crowd interaction, highly detailed textures, eye-catching composition, modern developer culture, celebration of software engineering and innovation."
- Append a concise description of the conference website theme gathered earlier to the prompt.
- Have Gemini generate the image from the completed prompt.
- Download the generated image and save it as "conference_presentation.png".
