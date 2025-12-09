---
agent: agent
---

Use playwright mcp tooling to perform the following steps, don't create any code and just execute the steps directly using the mcp tooling:

- navigate to the conference page: https://www.ittage.informatik-aktuell.de/index.html
- gather the website theme (e.g. fonts, ui style, colors, writing style) and (content) theme of the conference and save it for later use
- navigate to the google gemini page: https://gemini.google.com/app
- wait for the user to be logged in
- create a prompt for gemini instructing it to create an image with the style of the theme from the previous showing the following scenario:
  "Two male speaker with a "do epic shit" t-shirt in purble and Santa hats and sun glasses standing in front of an audience giving a presentation about the playwright tooling at a tech conference in Frankfurt/Main Germany. Christmas decorations are visible in the room and spread over the image as well as snow"
- append the filled prompt with a description of the theme of the conference page gathered earlier
- let gemini generate the image based on the created prompt
- finally download the generated image and save it as "conference_presentation.png"
