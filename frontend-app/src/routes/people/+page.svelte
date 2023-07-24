<script lang="ts">
	import LayoutPage from '$lib/layouts/LayoutPage.svelte';
	import { onMount } from 'svelte';
	import { writable } from 'svelte/store';
	import ApiHelpers from '../../services/ApiHelpers';
	import Loader from '$lib/components/Loader.svelte';
	import { toastStore, Avatar } from '@skeletonlabs/skeleton';
	import peopleApi, { type PersonResponseModel } from '../../services/PeopleApi';
	import Helpers from '../../lib/Helpers';

	let people = writable<PersonResponseModel[]>([]);
	let isCallingApi = writable<boolean>(false);

	async function deletePerson(id: number) {
		await peopleApi.deletePerson(id);
		people.set($people.filter((c) => c.id !== id));
	}

	onMount(async () => {
		isCallingApi.set(true);

		let response = await peopleApi.getPeople();
		if (!ApiHelpers.isErrorReponse(response)) {
			people.set(response);
		} else {
			toastStore.trigger({
				message: response.message,
				background: 'variant-filled-error'
			});
		}
		isCallingApi.set(false);
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
		{#if !$isCallingApi}
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
						{#if $people.length > 0}
							{#each $people as person}
								<tr>
									<td>
										<div class="flex flex-row items-center">
											<Avatar
												initials={Helpers.getInitials(person.name)}
												width="w-12"
											/>
											<div class="ml-4">{person.name}</div>
										</div>
									</td>
									<td>
										<div class="py-4">
											{person.email}
										</div>
									</td>
									<td>
										<div class="py-4">
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
										</div>
									</td>
								</tr>
							{/each}
						{:else}
							<tr>
								<td colspan="3">No Records Found</td>
							</tr>
						{/if}
					</tbody>
				</table>
			</div>
		{:else}
			<Loader />
		{/if}
	</section>
</LayoutPage>
