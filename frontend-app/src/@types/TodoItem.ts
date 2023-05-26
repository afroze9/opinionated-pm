export type TodoItem = {
  id: number;
  title: string;
  description: string;
  assignedToId: string;
  isCompleted: boolean;
}

export type UpdateTodoItemModel = {
  markComplete: boolean;
  assignedToId: string;
}

export type CreateTodoItemRequest = {
  title: string;
  description: string;
  assignedTo?: string;
}
