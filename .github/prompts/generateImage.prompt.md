---
agent: agent
---

Use playwright mcp tooling to perform the following steps, don't create any code and just execute the steps directly using the mcp tooling:

- navigate to the conference page: https://www.entwicklertag.de

- gather the website theme (e.g. fonts, ui style, colors, writing style) and (content) theme of the conference and save it for later use
- gather the faces of the speaker Harald Binkle and Nico Orschel from the conference speaker page on https://www.md-devdays.de/actor-overview and use it for later use in the image generation step (the speakers in the image should look like Harald and Nico)
- Save the gathered speaker profiles. upload the pictures before generating the image and use them as reference for the image generation step
- navigate to the google gemini page: https://gemini.google.com/app
- wait for the user to be logged in
- create a prompt for gemini instructing it to create an image with the style of the theme and speaker profile pictures from the previous showing the following scenario:
  "Two male tech speakers wearing stylish purple “Do Epic Shit” t-shirts and sunglasses are presenting Playwright tooling on stage at a vibrant developer conference in Karlsruhe, Germany, in June 2026.

The conference venue blends modern developer culture with the distinctive atmosphere of Karlsruhe, featuring contemporary architecture, innovative technology spaces, and subtle references to the city's unique fan-shaped urban layout radiating from Karlsruhe Palace. The environment combines the spirit of a leading German technology and innovation hub with a creative developer-community atmosphere.

A huge banner reading “Karlsruher Entwicklertag 2026” is displayed behind the stage. The audience is energetic and engaged, with a strong European developer community vibe. Warm early-summer sunlight streams through large windows, creating a bright May atmosphere typical for southwestern Germany. Outside, the city feels vibrant, innovative, and welcoming, with hints of Karlsruhe’s renowned tech ecosystem, research institutions, and startup culture.

The overall aesthetic combines a modern German tech-hub vibe with creative conference culture and subtle urban-art influences. The image should have a colorful neo-graffiti / urban mural style with spray paint textures, bold contrasts, cinematic lighting, dynamic composition, vivid purples and warm tones, highly detailed textures, and a visually striking festival-like energy.

Wide-angle shot, cinematic perspective, modern developer culture aesthetic, realistic crowd interaction, high detail, eye-catching composition, urban creative atmosphere, celebration of software engineering, testing, automation, and open-source innovation."
- append the filled prompt with a description of the theme of the conference page gathered earlier
- let gemini generate the image based on the created prompt
- finally download the generated image and save it as "conference_presentation.png"
