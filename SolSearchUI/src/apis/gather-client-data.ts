import type { PotentialClientData } from "../models/potentialClientData";

export async function getPotentialClients(
    locations: number[], enhanced : boolean = false): Promise<PotentialClientData[]> {

    const response = await fetch(`http://localhost:8080/api/v1/Conveyancing/GetPotentialClients?enhanced=${enhanced}`,
        {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(locations)
        }
    );

    if (!response.ok) {
        throw new Error("Failed to fetch potential clients");
    }

    return response.json();
}