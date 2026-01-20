/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./Areas/**/*.cshtml",
    "./Pages/**/*.cshtml"
  ],
  theme: {
    extend: {
      colors: {
        'sport-blue': '#51A2FF',
        'sport-purple': '#C27AFF',
        'sport-gradient-start': '#2B7FFF',
        'sport-gradient-end': '#9810FA',
      },
      fontFamily: {
        'arimo': ['Arimo', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
