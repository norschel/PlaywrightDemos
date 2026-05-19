---
agent: agent
---

Use playwright mcp tooling to perform the following steps, don't create any code and just execute the steps directly using the mcp tooling:

- navigate to the conference page: https://md-devdays.de/

- gather the website theme (e.g. fonts, ui style, colors, writing style) and (content) theme of the conference and save it for later use
- gather the faces of the speaker Harald Binkle and Nico Orschel from the conference speaker page on https://www.md-devdays.de/actor-overview and use it for later use in the image generation step (the speakers in the image should look like Harald and Nico)
- Save the gathered speaker profiles. upload the pictures before generating the image and use them as reference for the image generation step
- navigate to the google gemini page: https://gemini.google.com/app
- wait for the user to be logged in
- create a prompt for gemini instructing it to create an image with the style of the theme and speaker profile pictures from the previous showing the following scenario:
  "Two male tech speakers wearing stylish purple “Do Epic Shit” t-shirts and sunglasses are presenting Playwright tooling on stage at a vibrant developer conference in Magdeburg, Germany, in May 2026.

The conference venue blends modern developer culture with the unique Hundertwasser-inspired architecture of the Green Citadel of Magdeburg, featuring colorful organic shapes, artistic facades, curved lines, urban creative design elements, and a distinctive East German urban-tech atmosphere.

A huge banner reading “MD DevDays 2026” is displayed behind the stage. The audience is energetic and engaged, with a strong European developer community vibe. Warm early-summer sunlight streams through large windows, creating a bright May atmosphere typical for central Germany. Outside, the city feels lively, modern, and creative.

The overall aesthetic combines an East German urban-tech vibe with vibrant street art and modern conference culture. The image should have a colorful neo-graffiti / urban mural style with spray paint textures, bold contrasts, cinematic lighting, dynamic composition, vivid purples and warm tones, highly detailed textures, and a visually striking festival-like energy.

Wide-angle shot, cinematic perspective, modern developer culture aesthetic, realistic crowd interaction, high detail, eye-catching composition, urban creative atmosphere."
- append the filled prompt with a description of the theme of the conference page gathered earlier
- let gemini generate the image based on the created prompt
- finally download the generated image and save it as "conference_presentation.png"
