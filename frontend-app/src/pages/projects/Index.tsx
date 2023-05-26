import { Td, Container, Heading, Table, Thead, Tr, Th, Tbody, Button, Flex, Spacer, IconButton, Icon, Modal, ModalBody, ModalCloseButton, ModalContent, ModalFooter, ModalHeader, ModalOverlay, createDisclosure } from "@hope-ui/solid";
import { ColumnDef, createSolidTable, getCoreRowModel, flexRender } from "@tanstack/solid-table";
import { Component, For, createResource, createSignal, createEffect } from "solid-js";
import { ProjectResponse, TodoItem } from "../../@types";
import { Protected, useAuth0 } from "@afroze9/solid-auth0";
import { Link } from "@solidjs/router";
import { IconDelete, IconEdit } from "../../components/Icons";
import ProjectApi from "../../api/project/ProjectApi";
import { isErrorReponse } from "../../api/ErrorResponse";

const Projects: Component = () => {
  const auth0 = useAuth0();
  const { isOpen, onOpen, onClose } = createDisclosure();
  const [projects, setProjects] = createSignal<ProjectResponse[]>([]);
  const [projectToDelete, setProjectToDelete] = createSignal(0);

  const getProjectList = async (): Promise<ProjectResponse[]> => {
    const list = await ProjectApi.getProjects(await auth0.getToken());
    if (!isErrorReponse(list)) {
      return list as ProjectResponse[];
    }
    return [];
  }

  const defaultColumns: ColumnDef<ProjectResponse>[] = [
    {
      accessorKey: 'name',
      cell: info => <Td>{info.getValue<string>()}</Td>,
      footer: info => info.column.id,
    },
    {
      accessorKey: 'todoItems',
      cell: info => <Td>{info.getValue<TodoItem[]>()?.length ?? 0}</Td>,
      footer: info => info.column.id,
      header: 'Tasks'
    },
    {
      id: 'actions',
      cell: info => renderActions(info.row.original.id),
      footer: info => info.column.id
    }
  ];

  createEffect(async () => {
    try {
      let data = await getProjectList();
      setProjects(data);
    } catch (error) {
      console.error(error);
    }
  })


  const onDeleteClicked = (id: number) => {
    setProjectToDelete(id);
    onOpen();
  }

  async function onProjectDelete() {
    if (projectToDelete() !== 0) {
      await ProjectApi.deleteProject(projectToDelete(), await auth0.getToken());
      setProjects((prev) => {
        return prev.filter(c => c.id !== projectToDelete());
      });
    }

    onModalClose();
  }

  function onModalClose() {
    setProjectToDelete(0);
    onClose();
  }

  const renderActions = (id: number) => {
    const editUrl = `/projects/${id}/details`;
    return (
      <Flex>
        <IconButton aria-label="edit" icon={<IconEdit />} as={Link} href={editUrl}>Edit</IconButton>
        <IconButton
          css={{
            background: "$danger10",
            marginLeft: "$2",
            _hover: {
              background: "$danger11"
            }
          }}
          aria-label="delete" icon={<IconDelete />} onClick={() => onDeleteClicked(id)}>Edit</IconButton>
      </Flex>
    )
  }

  const table = createSolidTable<ProjectResponse>({
    get data() {
      return projects()
    },
    columns: defaultColumns,
    getCoreRowModel: getCoreRowModel(),
  })

  return (
    <Container p="$2">
      <Flex>
        <Heading size="xl">
          Projects
        </Heading>
        <Spacer />
        <Button as={Link} href='/projects/create'>Add Project</Button>
      </Flex>
      <Container mt="$4">
        <Table striped="odd" highlightOnHover>
          <Thead>
            <For each={table.getHeaderGroups()}>
              {headerGroup => (
                <Tr>
                  <For each={headerGroup.headers}>
                    {header => (
                      <Th>
                        {header.isPlaceholder
                          ? null
                          : flexRender(
                            header.column.columnDef.header,
                            header.getContext()
                          )}
                      </Th>
                    )}
                  </For>
                </Tr>
              )}
            </For>
          </Thead>
          <Tbody>
            <For each={table.getRowModel().rows}>
              {row => (
                <Tr>
                  <For each={row.getVisibleCells()}>
                    {cell => (
                      <td>
                        {flexRender(
                          cell.column.columnDef.cell,
                          cell.getContext()
                        )}
                      </td>
                    )}
                  </For>
                </Tr>
              )}
            </For>
          </Tbody>
        </Table>
      </Container>
      <Modal opened={isOpen()} onClose={onModalClose}>
        <ModalOverlay />
        <ModalContent>
          <ModalCloseButton />
          <ModalHeader>Delete Project</ModalHeader>
          <ModalBody>
            <p>Are you sure you want to delete this project?</p>
          </ModalBody>
          <ModalFooter>
            <Button onClick={onModalClose} mr="$3" variant="outline">Close</Button>
            <Button onClick={onProjectDelete} colorScheme="danger">Delete</Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </Container>
  );
};

export default () => (
  <Protected onRedirecting={<>Loading</>}>
    <Projects />
  </Protected>
)
