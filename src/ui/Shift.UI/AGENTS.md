# Projects Guidelines

## Components

- **Child Component** - Name the child component using pattern "ParentComponentName"_"ChildComponentName"

- **FormField** - Use `FormField` component when `form-group` div is used.
  For example, instead of this:
  ```
  <div className="form-group mb-3 some-class">
      <label className="form-label">
          Question Flag
      </label>
      <TextBox />
      <div className="form-text">
          Some description
      </div>
  </div>
  ```
  use this:
  ```
  <FormField label="Question Flag" description="Some description" className="some-class">
      <TextBox />
  </FormField>
  ```

## Coding style and rules

- **General code style** - Use the same code styling as the rest of the project
- **Event handlers** - Use short syntax for one line event handlers, regardles if it sync or async, for example, use this pattern:
  ```
  onSave={value => saveQuestionTitle(question, value)}
  ```
- **Icons** - use `Icon` component to display fontawesome icons.
  If the icon name does not exist then add it to `IconName`, names in the union should be placed in alphabetical order.

- **Decimal numbers** - use `numberHelper.formatDecimal` to format decimal numbers.
  Example of decimal numbers: Points, Scores.

## API usage rules

- **Decimal numbers** - API sends decimal numbers as `sentDecimal = (originalDecimal * 10,000) as integer`

## CSS instructions

- Use CSS nesting syntax while keeping CSS files as `.css`