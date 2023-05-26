import { describe, expect, test } from 'vitest'
import { render } from "@solidjs/testing-library"
import { ProjectCard } from "../../src/components/ProjectCard";
import { HopeProvider } from '@hope-ui/solid';
import { Router } from '@solidjs/router';

describe("ProjectCard", () => {
  const project = {
    id: 1,
    name: "Project 1",
    tasks: 10,
  };

  test("renders the project name", () => {
    const { queryByText } = render(() => <Router><HopeProvider><ProjectCard {...project} /></HopeProvider></Router>);
    expect(queryByText('Project 1')).toBeDefined();
  });

  test("renders the number of tasks", () => {
    const { queryByText } = render(() => <Router><HopeProvider><ProjectCard {...project} /></HopeProvider></Router>);
    expect(queryByText('Tasks: 10')).toBeDefined();
  });

  test("renders a link to view project details", () => {
    const { queryByText } = render(() => <Router><HopeProvider><ProjectCard {...project} /></HopeProvider></Router>);
    expect(queryByText("View")).toHaveAttribute("href", "/projects/1/details");
  });

  test('matches snapshot', () => {
    const { container, queryByText } = render(() => <Router><HopeProvider><ProjectCard {...project} /></HopeProvider></Router>);
    expect(container.innerHTML).toMatchSnapshot();
  });

});
