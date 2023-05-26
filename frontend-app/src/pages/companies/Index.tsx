import { Button, Container, createDisclosure, Flex, Heading, IconButton, Modal, ModalBody, ModalCloseButton, ModalContent, ModalFooter, ModalHeader, ModalOverlay, Spacer, Table, Tbody, Td, Th, Thead, Tr } from "@hope-ui/solid";
import { Component, For, createSignal, createEffect } from "solid-js";
import { ColumnDef, createSolidTable, flexRender, getCoreRowModel } from "@tanstack/solid-table";
import { CompanySummaryResponseModel } from "../../@types";
import { Link } from "@solidjs/router";
import { IconEdit, IconDelete } from "../../components/Icons";
import { Protected, useAuth0 } from "@afroze9/solid-auth0";
import CompanyApi from "../../api/company/CompanyApi";
import { isErrorReponse } from "../../api/ErrorResponse";


const Companies: Component = () => {
  const auth0 = useAuth0();
  const { isOpen, onOpen, onClose } = createDisclosure();
  const [companies, setCompanies] = createSignal<CompanySummaryResponseModel[]>([]);
  const [companyToDelete, setCompanyToDelete] = createSignal(0);

  const getCompanyList = async (): Promise<CompanySummaryResponseModel[]> => {
    const list = await CompanyApi.getCompanies(await auth0.getToken());
    if (!isErrorReponse(list)) {
      return list as CompanySummaryResponseModel[];
    }
    return [];
  }

  const defaultColumns: ColumnDef<CompanySummaryResponseModel>[] = [
    {
      accessorKey: 'name',
      cell: info => <Td>{info.getValue<string>()}</Td>,
      footer: info => info.column.id,
    },
    {
      accessorKey: 'projectCount',
      cell: info => <Td>{info.getValue<number>()}</Td>,
      footer: info => info.column.id,
      header: 'Projects'
    },
    {
      id: 'actions',
      cell: info => renderActions(info.row.original.id),
      footer: info => info.column.id
    }
  ]

  createEffect(async () => {
    try {
      const data = await getCompanyList();
      setCompanies(data);
    } catch (error) {
      console.error(error);
    }
  })

  const onDeleteClicked = (id: number) => {
    setCompanyToDelete(id);
    onOpen();
  }

  async function onCompanyDelete() {
    if (companyToDelete() !== 0) {
      await CompanyApi.deleteCompany(companyToDelete(), await auth0.getToken());
      setCompanies((prev) => {
        return prev.filter(c => c.id !== companyToDelete());
      });
    }

    onModalClose();
  }

  function onModalClose() {
    setCompanyToDelete(0);
    onClose();
  }

  const renderActions = (id: number) => {
    const editUrl = `/companies/${id}/details`;
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

  const table = createSolidTable<CompanySummaryResponseModel>({
    get data() {
      return companies()
    },
    columns: defaultColumns,
    getCoreRowModel: getCoreRowModel(),
  })

  return (
    <Container p="$2">
      <Flex>
        <Heading size="xl">
          Companies
        </Heading>
        <Spacer />
        <Button as={Link} href='/companies/create' >Add Company</Button>
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
          <ModalHeader>Delete Company</ModalHeader>
          <ModalBody>
            <p>Are you sure you want to delete this company?</p>
          </ModalBody>
          <ModalFooter>
            <Button onClick={onModalClose} mr="$3" variant="outline">Close</Button>
            <Button onClick={onCompanyDelete} colorScheme="danger">Delete</Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </Container>
  );
};

export default () => (
  <Protected onRedirecting={<>Loading</>}>
    <Companies />
  </Protected>
)
