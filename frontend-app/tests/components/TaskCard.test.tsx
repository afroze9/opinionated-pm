import { describe, expect, test } from 'vitest'
import { render } from "@solidjs/testing-library"
import { TaskCard } from "../../src/components/TaskCard";
import { HopeProvider } from '@hope-ui/solid';
import { Router } from '@solidjs/router';

describe("TaskCard", () => {
  const Task = {
    id: 1,
    title: "Task 1",
    description: "Task description",
    isCompleted: false,
    onComplete: async (id: number) => console.log('hello')
  };

  test("renders the Task title", () => {
    const { queryByText } = render(() => <Router><HopeProvider><TaskCard {...Task} /></HopeProvider></Router>);
    expect(queryByText('Task 1')).toBeDefined();
  });

  test("renders the Task description", () => {
    const { queryByText } = render(() => <Router><HopeProvider><TaskCard {...Task} /></HopeProvider></Router>);
    expect(queryByText('Task description')).toBeDefined();
  });

  test('matches snapshot', () => {
    const { container, queryByText } = render(() => <Router><HopeProvider><TaskCard {...Task} /></HopeProvider></Router>);
    expect(container.innerHTML).toMatchSnapshot();
  });

});
