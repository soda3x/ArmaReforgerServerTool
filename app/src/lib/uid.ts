// Generates stable, unique DOM ids so reusable field components can associate their <label>
// with their input (screen readers rely on that association; a bare <label> announces nothing).

let counter = 0;

export function uid(prefix = "f"): string {
  counter += 1;
  return `${prefix}-${counter}`;
}
