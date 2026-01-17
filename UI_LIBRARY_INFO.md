# UI Library Information

## Primary UI Framework

This repository uses **Bootstrap** as its primary UI library/framework.

## Details

### Framework Information
- **Framework**: Bootstrap
- **Type**: CSS Framework
- **License**: MIT License (Copyright (c) 2011-2021 Twitter, Inc. and The Bootstrap Authors)

### Location
Bootstrap files are located in:
```
wwwroot/lib/bootstrap/
├── dist/
│   ├── css/
│   │   └── bootstrap.min.css
│   └── js/
│       └── bootstrap.bundle.min.js
└── LICENSE
```

### Implementation
Bootstrap is referenced in the main layout file at `Views/Shared/_Layout.cshtml`:

```html
<!-- CSS -->
<link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />

<!-- JavaScript -->
<script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
```

### Bootstrap Usage in Views
The application extensively uses Bootstrap classes throughout the views, including:
- Layout utilities: `container`, `container-xl`, `d-flex`, `justify-content-between`
- Spacing: `py-2`, `py-4`, `pb-3`, `gap-2`
- Typography: `fs-5`, `fw-bold`, `text-white`, `text-muted`
- Components: `navbar-nav`, `sticky-top`, `rounded`
- Grid system: Bootstrap's responsive grid classes
- Theme utilities: `bg-black`, `bg-primary`, `theme-dark`

### Additional Front-end Libraries
The application also includes:
1. **jQuery** (`wwwroot/lib/jquery/`)
   - JavaScript library for DOM manipulation
   
2. **jQuery Validation** (`wwwroot/lib/jquery-validation/`)
   - Client-side form validation
   
3. **jQuery Validation Unobtrusive** (`wwwroot/lib/jquery-validation-unobtrusive/`)
   - Integration with ASP.NET Core MVC validation

## Project Type
This is an **ASP.NET Core MVC** web application (targeting .NET 10.0) that uses:
- **Razor Views** (.cshtml files) for server-side rendering
- **Bootstrap** for responsive UI design and styling
- **jQuery** for client-side interactions

## Summary
**Bootstrap** is the main UI library used for styling and responsive design in this SportMania application.
