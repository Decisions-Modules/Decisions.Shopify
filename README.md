# Decisions.Shopify

This module provides Shopify Admin API steps for:
- Products
- Inventory / Variants
- Orders
- Customers

## Authentication and Settings

This module does not use a module settings object.

Each step receives connection inputs directly:
- `Store Domain` (for example `my-store.myshopify.com`)
- `Token` (Decisions OAuth token selected with token picker)

The module API version is hardcoded to `2026-01`.

## Required OAuth Scopes

Ensure your Shopify app token includes scopes needed by the steps you use.

Recommended baseline for current steps:
- Products: `read_products`, `write_products`
- Inventory/Variants: `read_inventory`, `write_inventory`, plus product read access for variant lookups
- Orders: `read_orders`
- Customers: `read_customers`

## Step Catalog

### Integration/Shopify/Products

#### Get Product By Id
Intention:
- Fetch one product by Shopify GraphQL product ID.

How to use:
1. Provide `Store Domain` and `Token`.
2. Provide `productId` (GraphQL global ID such as `gid://shopify/Product/...`).
3. Use returned typed `ShopifyProduct` fields in subsequent flow logic.

#### List Products
Intention:
- Retrieve a page of products for browse/sync scenarios.

How to use:
1. Provide `Store Domain` and `Token`.
2. Set `first` page size.
3. Optional `after` cursor for next-page retrieval.
4. Read `HasNextPage` and `EndCursor` from `ShopifyProductsPage` to paginate.

#### Create Product
Intention:
- Create a product using typed product input.

How to use:
1. Provide `Store Domain` and `Token`.
2. Set `ShopifyProductInput` (title required, optional handle/status/vendor/productType).
3. Execute step and inspect `ShopifyMutationResult<ShopifyProduct>`.
4. If `UserErrors` is not empty, handle validation/business errors in flow.

#### Update Product
Intention:
- Update an existing product by ID.

How to use:
1. Provide `Store Domain` and `Token`.
2. Provide `productId` and `ShopifyProductInput` fields to update.
3. Inspect `Data` for updated product.
4. Check `UserErrors` for Shopify validation failures.

#### Delete Product
Intention:
- Delete a product by ID.

How to use:
1. Provide `Store Domain` and `Token`.
2. Provide `productId`.
3. Inspect `ShopifyMutationResult<bool>.Data` for success.
4. Handle `UserErrors` if delete was rejected.

### Integration/Shopify/Inventory

#### List Variants By Product Id
Intention:
- Return typed variant data for a product, including inventory item references and per-location inventory levels.

How to use:
1. Provide `Store Domain` and `Token`.
2. Provide `productId`.
3. Set `first` to control returned count.
4. Use `InventoryItemId` from each variant for inventory adjustments.
5. Use `InventoryLevels[].LocationId` when you need a valid location for adjustment.

#### List Locations
Intention:
- Retrieve Shopify locations to drive location selection in inventory flows.

How to use:
1. Provide `Store Domain` and `Token`.
2. Set `first` page size; optional `after` cursor for next page.
3. Use returned `Id` as `locationId` for inventory adjustment.
4. Use `Name` and `IsActive` for UI filtering or operator choice.

#### Adjust Inventory Available
Intention:
- Apply an inventory delta for an inventory item at a location.

How to use:
1. Provide `Store Domain` and `Token`.
2. Provide `inventoryItemId`, `locationId`, and integer `delta`.
3. Positive delta increments available quantity; negative delta decrements.
4. Inspect `UserErrors` for authorization, validation, or domain errors.

### Integration/Shopify/Orders

#### Get Order By Id
Intention:
- Fetch one order with key financial/fulfillment summary fields.

How to use:
1. Provide `Store Domain` and `Token`.
2. Provide GraphQL `orderId`.
3. Consume typed `ShopifyOrder` output.

#### List Orders
Intention:
- Query orders with cursor pagination and optional Shopify search filter.

How to use:
1. Provide `Store Domain` and `Token`.
2. Set `first` page size.
3. Optional `after` cursor.
4. Optional `queryFilter` using Shopify order query syntax.
5. Loop while `HasNextPage` is true.

### Integration/Shopify/Customers

#### Get Customer By Id
Intention:
- Fetch one customer by GraphQL customer ID.

How to use:
1. Provide `Store Domain` and `Token`.
2. Provide `customerId`.
3. Use typed `ShopifyCustomer` fields in downstream steps.

#### List Customers
Intention:
- Query customers with cursor pagination and optional search filter.

How to use:
1. Provide `Store Domain` and `Token`.
2. Set `first` page size.
3. Optional `after` cursor.
4. Optional `queryFilter` using Shopify customer search syntax.
5. Continue paging using `HasNextPage` and `EndCursor`.

## Quick Start Flow Example

This example shows a common inventory workflow:
1. List products.
2. Select one product.
3. List variants for that product.
4. Adjust inventory for one variant at one location.

### Prerequisites
- You have store domain and a Decisions OAuth token to select on each step.
- Token has product and inventory scopes.
- You can get location IDs from `List Variants By Product Id` (`InventoryLevels`) or `List Locations`.

### Flow Outline

#### Step 1: List Products
- Use `Integration/Shopify/Products -> List Products`.
- Set `first` (for example `25`), leave `after` empty on first call.
- Select a product from `Products` output.

#### Step 2: List Variants By Product Id
- Use `Integration/Shopify/Inventory -> List Variants By Product Id`.
- Pass selected product `Id` and set `first` (for example `50`).
- Select target variant and capture `InventoryItemId`.
- Choose a `LocationId` from the variant `InventoryLevels`.

#### Optional Step: List Locations
- Use `Integration/Shopify/Inventory -> List Locations`.
- Use this when you want a store-wide list of locations instead of variant-scoped locations.
- Pick target location `Id` for the adjustment step.

#### Step 3: Adjust Inventory Available
- Use `Integration/Shopify/Inventory -> Adjust Inventory Available`.
- Inputs:
	- `inventoryItemId`: from selected variant
	- `locationId`: from `InventoryLevels` or `List Locations`
	- `delta`: integer quantity change (positive adds, negative removes)
- Check `UserErrors`; if not empty, route to error handling path.

### Optional Multi-Store Pattern
- Since connection values are step inputs, multi-store routing is native: pass different `Store Domain` and `Token` values per execution path.
