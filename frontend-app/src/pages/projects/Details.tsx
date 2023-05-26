import { Protected, useAuth0 } from "@afroze9/solid-auth0";
import { Button, Container, Flex, FormControl, FormErrorMessage, FormLabel, Heading, HStack, Input, SelectOptionIndicator, SelectOptionText, SimpleOption, SimpleSelect, VStack } from "@hope-ui/solid";
import { Link, useNavigate, useParams } from "@solidjs/router";
import { Component, createEffect, createSignal, For } from "solid-js";
import { CreateTodoItemRequest, Priority, ProjectResponse, TodoItem } from "../../@types";
import ProjectApi from "../../api/project/ProjectApi";
import { ErrorResponse, isErrorReponse } from "../../api/ErrorResponse";
import { InferType, number, object, string } from "yup";
import { createForm } from "@felte/solid";
import { validator } from "@felte/validator-yup";
import { TaskCard } from "../../components/TaskCard";
import TodoApi from "../../api/project/TodoApi";

type ProjectFormProps = {
  name: string;
  priority: number;
};

type TaskFormProps = {
  title: string;
  description: string;
}

const projectSchema = object({
  name: string().min(5).required(),
  priority: number().required(),
});

const taskSchema = object({
  title: string().min(5).required(),
  description: string().min(5).required(),
});


const ProjectDetails: Component = () => {
  const auth0 = useAuth0();
  const params = useParams();
  const navigate = useNavigate();

  const [project, setProject] = createSignal<ProjectResponse>({
    id: 0,
    name: '',
    todoItems: [],
    companyId: 0,
    priority: Priority.Medium,
  });

  const [task, setTask] = createSignal<CreateTodoItemRequest>({
    title: '',
    description: ''
  });

  createEffect(async () => {
    try {
      let data = await getProject(+params.id);
      setProject(data);
      projectForm.setFields({
        name: data.name,
        priority: data.priority,
      })
    } catch (error) {
      console.error(error);
    }
  });

  const projectForm = createForm<InferType<typeof projectSchema>>({
    extend: validator({ schema: projectSchema }),
    initialValues: {
      name: '',
      priority: 2,
    },
    onSubmit: values => {
      saveProject(values);
    },
  });

  const taskForm = createForm<InferType<typeof taskSchema>>({
    extend: validator({ schema: taskSchema }),
    initialValues: {
      title: '',
      description: '',
    },
    onSubmit: values => {
      createTask(values);
    },
  });

  async function getProject(id: number): Promise<ProjectResponse> {
    const response = await ProjectApi.getProjectById(id, await auth0.getToken());
    if (!isErrorReponse(response)) {
      let project = response as ProjectResponse;
      return project;
    }

    return {
      id: 0,
      name: '',
      todoItems: [],
      companyId: 0,
      priority: Priority.Medium,
    }
  }

  async function saveProject(values: ProjectFormProps) {
    let updatedProject = {
      id: +params.id,
      name: values.name,
      priority: values.priority,
      companyId: project().companyId,
    };

    let response = await ProjectApi.updateProject(+params.id, updatedProject, await auth0.getToken());
    if (!isErrorReponse(response)) {
      navigate('/projects');
    } else {
      alert((response as ErrorResponse).message);
    }
  }

  async function onTaskComplete(taskId: number) {
    let updatedTodo = {
      markComplete: true,
      assignedToId: '',
    };

    let response = await TodoApi.updateTodo(taskId, updatedTodo, await auth0.getToken());
    if (!isErrorReponse(response)) {
      const updatedTasks = project().todoItems.map(item => {
        if (item.id === taskId) {
          return {
            ...item,
            isCompleted: true,
          }
        }
        return item;
      });

      setProject((prev) => {
        return {
          ...prev,
          todoItems: updatedTasks
        }
      });
    }
  }

  async function createTask(values: TaskFormProps) {
    let taskToCreate = {
      title: values.title,
      description: values.description,
    };

    let response = await TodoApi.createTodo(+params.id, taskToCreate, await auth0.getToken());
    if (!isErrorReponse(response)) {

      setProject((prev) => {
        return {
          ...prev,
          todoItems: [...prev.todoItems, response as TodoItem]
        }
      })

      setTask({
        title: '',
        description: ''
      });
      taskForm.setFields({
        title: '',
        description: ''
      });
      taskForm.setIsDirty(false);
      taskForm.setTouched({
        title: false,
        description: false
      })
    }
  }

  function navigateBack() {
    navigate(-1);
  }

  return (
    <Container p="$2">
      <Flex>
        <Heading size="xl">
          {project()?.name}
        </Heading>
      </Flex>
      <Container mt="$4">
        <VStack
          as='form'
          ref={projectForm.form}
          spacing="$5"
          alignItems="stretch"
          maxW="$128"
          mx="auto"
        >
          <FormControl required invalid={!!projectForm.errors("name")}>
            <FormLabel>Name</FormLabel>
            <Input type="text" name="name" placeholder="Project Name" value={project()?.name} />
            <FormErrorMessage>{projectForm.errors("name")?.[0]}</FormErrorMessage>
          </FormControl>

          <FormControl required invalid={!!projectForm.errors("priority")}>
            <FormLabel for="priority">Priority</FormLabel>
            <SimpleSelect
              value={project()?.priority}
              onChange={value => projectForm.setFields("priority", value)}
            >
              <SimpleOption value={1}><SelectOptionText>Low</SelectOptionText><SelectOptionIndicator /></SimpleOption>
              <SimpleOption value={2}><SelectOptionText>Medium</SelectOptionText><SelectOptionIndicator /></SimpleOption>
              <SimpleOption value={3}><SelectOptionText>High</SelectOptionText><SelectOptionIndicator /></SimpleOption>
              <SimpleOption value={4}><SelectOptionText>Critical</SelectOptionText><SelectOptionIndicator /></SimpleOption>
            </SimpleSelect>
            <FormErrorMessage>{projectForm.errors("priority")?.[0]}</FormErrorMessage>
          </FormControl>

          <HStack justifyContent="flex-end" spacing="$5">
            <Button type="button" colorScheme="danger" onclick={navigateBack}>
              Cancel
            </Button>
            <Button type="submit" disabled={!projectForm.isValid()}>
              Update
            </Button>
          </HStack>
        </VStack>
      </Container>

      <Flex mt="$4">
        <Heading size="xl">
          Tasks
        </Heading>
      </Flex>

      <Container mt="$4">
        <HStack
          as='form'
          ref={taskForm.form}
          spacing="$5"
          alignItems="stretch"
          maxW="$128"
          mx="auto"
        >
          <FormControl required invalid={!!taskForm.errors("title")}>
            <Input type="text" name="title" placeholder="Title" value={task()?.title} />
            <FormErrorMessage>{taskForm.errors("title")?.[0]}</FormErrorMessage>
          </FormControl>
          <FormControl required invalid={!!taskForm.errors("description")}>
            <Input type="text" name="description" placeholder="Description" value={task()?.description} />
            <FormErrorMessage>{taskForm.errors("description")?.[0]}</FormErrorMessage>
          </FormControl>
          <Button type="submit" disabled={!taskForm.isValid()} >
            Add
          </Button>
        </HStack>
      </Container>

      <Container mt="$4">
        <VStack spacing="$4">
          <For each={project()?.todoItems}>
            {item => (
              <TaskCard
                id={item.id}
                title={item.title}
                description={item.description}
                isCompleted={item.isCompleted}
                onComplete={onTaskComplete}
              />
            )}
          </For>
        </VStack>
      </Container>
    </Container>
  );
}

export default () => (
  <Protected onRedirecting={<>Loading</>}>
    <ProjectDetails />
  </Protected>
)
