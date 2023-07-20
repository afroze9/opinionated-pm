<script lang="ts">
    import LayoutPage from '$lib/layouts/LayoutPage.svelte';
    import companyApi from '../../services/CompanyApi';
    import { onMount } from 'svelte';
    import { writable } from 'svelte/store';
    import ApiHelpers from '../../services/ApiHelpers';
    import Loader from '$lib/components/Loader.svelte';
    import { toastStore, type ToastSettings } from '@skeletonlabs/skeleton';
    import peopleApi, {type PersonResponseModel} from "../../services/PeopleApi";

    let people = writable<PersonResponseModel[]>([]);

    async function deletePerson(id: number) {
        await companyApi.deleteCompany(id);
        people.set($people.filter((c) => c.id !== id));
    }

    onMount(async () => {
        let response = await peopleApi.getPeople();

        if (!ApiHelpers.isErrorReponse(response)) {
            people.set(response);
        } else {
            toastStore.trigger({
                message: response.message,
                background: 'variant-filled-error'
            });
        }
    });
</script>

<LayoutPage>
    <header class="space-y-4">
        <div class="flex flex-row">
            <h1 class="h1">People</h1>
            <a href="/people/create" class="btn variant-filled ml-auto">Add User</a>
        </div>
    </header>
    <hr />
    <section class="space-y-4">
        {#if $people.length > 0}
            <div class="table-container">
                <table class="table" role="grid">
                    <thead class="table-head">
                    <tr>
                        <th>Name</th>
                        <th>Email</th>
                        <th />
                    </tr>
                    </thead>
                    <tbody class="table-body">
                    {#each $people as person}
                        <tr>
                            <td>{person.name}</td>
                            <td>{person.email}</td>
                            <td>
                                <a href={`people/${person.id}`}>
                                    <i class="fa-solid fa-pencil" />
                                </a>
                                <button
                                        on:click={() => deletePerson(person.id)}
                                        on:keypress={() => deletePerson(person.id)}
                                        class="ml-2 text-error-500"
                                >
                                    <i class="fa-solid fa-trash" />
                                </button>
                            </td>
                        </tr>
                    {/each}
                    </tbody>
                </table>
            </div>
        {:else}
            <Loader />
        {/if}
    </section>
</LayoutPage>
