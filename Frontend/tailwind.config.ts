import type { Config } from 'tailwindcss';

const config: Config = {
  content: [
    './src/**/*.{js,ts,jsx,tsx,mdx}',
  ],
  theme: {
    extend: {
      colors: {
        'sm-bg': '#0d1117',
        'sm-card': '#161b22',
        'sm-border': '#30363d',
        'sm-text': '#c9d1d9',
        'sm-text-light': '#f0f6fc',
        'sm-muted': '#8b949e',
        'sm-hover': '#484f58',
        'sm-primary': '#0d6efd',
        'sm-btn-sec': '#21262d',
        'sm-btn-sec-border': '#30363d',
      },
    },
  },
  plugins: [],
};

export default config;
